# Import scripts
. .\build.github.ps1
. .\build.utils.ps1

$Root = $PSScriptRoot

Properties {
    $Configuration      = "Release"
    $Name               = "Hadouken"
    $SolutionFile       = "Hadouken.sln"
    $AssemblyInfo       = "src/CommonAssemblyInfo.cs"

    # Artifacts
    $Artifact_Choco     = "hadouken.$Version.nupkg"
    $Artifact_Zip       = "hadouken-$Version.zip"
    $Artifact_Msi       = "hadouken-$Version.msi"

    # Directories
    $Dir_Artifacts      = Join-Path $Root "build"
    $Dir_Binaries       = Join-Path $Root "build/bin"
    $Dir_Output         = Join-Path $Root "src/Hadouken/bin/$Configuration/"

    # GitHub settings
    $GitHub_Owner       = "hadouken"
    $GitHub_Repository  = "hadouken"

    # Chocolatey
    $Chocolatey_Source  = "http://chocolatey.org/"

    # Tools
    $Tools_7za          = Join-Path $Root "tools/7za-9.20/7za.exe"
    $Tools_NuGet        = Join-Path $Root "tools/nuget.exe"
    $Tools_WixCandle    = Join-Path $Root "packages\WiX.Toolset.3.8.1128.0\tools\wix\candle.exe"
    $Tools_WixLight     = Join-Path $Root "packages\WiX.Toolset.3.8.1128.0\tools\wix\light.exe"
    $Tools_xUnit        = Join-Path $Root "packages\xunit.runners.1.9.2\tools\xunit.console.clr4.x86.exe"
}

$h = Get-Host
$w = $h.UI.RawUI.WindowSize.Width

FormatTaskName (("-"*$w) + "`r`n[{0}]`r`n" + ("-"*$w))

Task Default -depends Clean, Prepare, Compile, Test, Output, Zip, MSI, Chocolatey
Task Publish -depends Publish-GitHub, Publish-Chocolatey

Task Clean {
    Write-Host "Cleaning and creating build artifacts folder."

    If (Test-Path $Dir_Artifacts)
    {
        Remove-Item $Dir_Artifacts -Recurse -Force | Out-Null
    }

    New-Item $Dir_Artifacts -ItemType directory | Out-Null
    New-Item $Dir_Binaries  -ItemType directory | Out-Null

    If (Test-Path $AssemblyInfo) {
        Remove-Item $AssemblyInfo
    }

    Exec { msbuild $SolutionFile /t:Clean "/p:Configuration=$Configuration" /v:quiet } 
}

Task Prepare -depends Clean {
    If (Test-Command git) {
        # Get branch name
        If (!$BranchName) {
            $BranchName = (git symbolic-ref HEAD)
        }

        # Get commit
        if (!$Commit) {
            $Commit = (git rev-parse HEAD)

            (git diff-files --quiet)
            if($LASTEXITCODE -ne 0) {
                $Commit = $Commit + "*"
            }
        }
    }

    Generate-Assembly-Info -file $AssemblyInfo `
                           -branchName $BranchName `
                           -buildDate ([System.DateTime]::UtcNow).ToString("yyyy-MM-ddTHH\:mm\:ss.fffffffzzz") `
                           -commit $Commit `
                           -company "Viktor Elofsson - viktorelofsson.se" `
                           -product "Hadouken" `
                           -title "Hadouken" `
                           -description "A headless BitTorrent client for Windows." `
                           -version $Version
}

Task Compile -depends Prepare {
    Exec { msbuild $SolutionFile /t:Build "/p:Configuration=$Configuration" /v:quiet }
}

Task Test -depends Compile {
    Exec { & $Tools_xUnit "./src/Hadouken.Common.Tests/bin/$Configuration/Hadouken.Common.Tests.dll" }
    Exec { & $Tools_xUnit "./src/Hadouken.Core.Tests/bin/$Configuration/Hadouken.Core.Tests.dll" }
}

Task Output -depends Compile {
    $filter = ("*.dll", "*.exe")

    Write-Host "Copying files from $Dir_Output to $Dir_Binaries"

    # Copy files
    Get-ChildItem -Path $Dir_Output -Include $filter -Recurse |
        Copy-Item -Destination {
            If ($_.PSIsContainer) {
                Join-Path $Dir_Binaries $_.Parent.FullName.Substring($Dir_Output.Length)
            } Else {
                $path = (Split-Path $_.FullName)

                If ($path.Length -ge $Dir_Output.Length) {
                    $sub = Join-Path $Dir_Binaries $path.Substring($Dir_Output.Length)
                    New-Item -ItemType Directory -Path $sub | Out-Null
                }

                Join-Path $Dir_Binaries $_.FullName.Substring($Dir_Output.Length)
            }
        } -Force

    # Copy the correct config file
    # Copy-Item -Path .\src\Configuration\Console\Hadouken.Service.exe.config -Destination $Dir_Binaries

    # Zip the web UI
    Write-Host "Compressing and packaging the web ui"
    $webuiZip = Join-Path $Dir_Binaries "Web/web.zip"
    Exec {
        & $Tools_7za a -mx=9 $webuiZip .\src\Web\*
    }
}

Task Zip -depends Output {
    $files = "$Dir_Binaries/*"
    $output = Join-Path $Dir_Artifacts $Artifact_Zip

    Exec {
        & $Tools_7za a -mx=9 $output $files
    }
}

Task MSI -depends Output {
    $wxs = Get-ChildItem "src\Installer\" -Include *.wxs -recurse | Select-Object FullName | foreach {$_.FullName}

    Exec {
        & $Tools_WixCandle "-dBinDir=$Dir_Binaries" "-dBuildVersion=$Version" -dConfigDir=src\Configuration\Service -ext WixUtilExtension -o "$Dir_Artifacts\wixobj\" $wxs
    }

    $wixobj = Get-ChildItem "$Dir_Artifacts\wixobj\" -Include *.wixobj -Recurse | Select-Object FullName | foreach {$_.FullName}
    $msi = Join-Path $Dir_Artifacts $Artifact_Msi

    Exec {
        & $Tools_WixLight -ext WixUIExtension -ext WixUtilExtension -ext WixNetFxExtension -sval -o $msi $wixobj
    }
}

Task Chocolatey -depends Output {
    $source = Join-Path $Root "src/Chocolatey/"
    $destination = Join-Path $Dir_Artifacts "choco/"

    Copy-Item -Path $source -Destination $destination -Recurse -Container -Force

    # Set URL in chocolateyInstall.ps1'
    $chocoInstall = Join-Path $destination "tools/chocolateyInstall.ps1"
    $content = Get-Content $chocoInstall
    $content = $content.Replace("%DOWNLOAD_URL%", "http://www.hdkn.net/download/v$($Version)?source=chocolatey")

    Set-Content $chocoInstall $content

    Exec {
        & $Tools_NuGet pack "$Dir_Artifacts/choco/Hadouken.nuspec" -Version $Version -NoPackageAnalysis -OutputDirectory $Dir_Artifacts
    }
}

Task Publish-GitHub -depends MSI, Zip {
    $data = @{
        tag_name         = "v$Version"
        target_commitish = "master"
        name             = "$Name $Version"
    }

    Write-Host "Creating release $($data.tag_name)"

    $release = New-GitHubRelease "https://api.github.com/repos/$GitHub_Owner/$GitHub_Repository/releases" $data $GitHub_Token
    $releaseMSI = Join-Path $Dir_Artifacts $Artifact_Zip
    $releaseZip = Join-Path $Dir_Artifacts $Artifact_Msi

    Write-Host "Uploading release assets"

    Upload-GitHubReleaseAsset $GitHub_Token $GitHub_Owner $GitHub_Repository $release.Id $releaseMSI "application/octet-stream" | Out-Null
    Upload-GitHubReleaseAsset $GitHub_Token $GitHub_Owner $GitHub_Repository $release.Id $releaseZip "application/zip" | Out-Null
}

Task Publish-Chocolatey -depends Chocolatey {
    Write-Host "Pushing Hadouken to Chocolatey"

    $pkg = Join-Path $Dir_Artifacts $Artifact_Choco
    Exec {
        & $Tools_NuGet push $pkg $Chocolatey_API_Key -Source $Chocolatey_Source
    }
}