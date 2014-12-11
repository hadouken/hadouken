# Import scripts
. .\build.github.ps1
. .\build.utils.ps1

$Root = $PSScriptRoot

Properties {
    $Configuration      = "Release"
    $Name               = "Hadouken"
    $SolutionFile       = "Hadouken.sln"
    $AssemblyInfo       = "src/CommonAssemblyInfo.cs"
    $Commit             = $null

    # Artifacts
    $Artifact_Choco     = "hadouken.$BuildVersion.nupkg"
    $Artifact_Zip       = "hadouken-$BuildVersion.zip"
    $Artifact_Msi       = "hadouken-$BuildVersion.msi"

    # Directories
    $Dir_Artifacts      = Join-Path $Root "build"
    $Dir_Binaries       = Join-Path $Root "build/bin"
    $Dir_Output         = Join-Path $Root "src/Core/Hadouken/bin/x86/$Configuration/"

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

FormatTaskName {
    param($taskName)

    Write-Host "$taskName" -ForegroundColor Cyan

    if($env:APPVEYOR) {
        Add-AppveyorMessage "Running task: $taskName"
    }
}

Task Default -depends Prepare, Clean, Generate-CommonAssemblyInfo, Compile, Test, Output, Zip, MSI, Chocolatey, Publish-AppVeyor
Task Publish -depends Default, Publish-GitHub, Publish-Chocolatey

Task Prepare {
    # Update version
    if($env:APPVEYOR) {
        Write-BuildMessage "Updating AppVeyor version to $BuildVersion"
        Update-AppveyorBuild -Version $BuildVersion
    }
}

Task Clean -depends Prepare {
    Write-BuildMessage "Cleaning and build output folder."

    If (Test-Path $Dir_Artifacts)
    {
        Remove-Item $Dir_Artifacts -Recurse -Force | Out-Null
    }

    New-Item $Dir_Artifacts -ItemType directory | Out-Null
    New-Item $Dir_Binaries  -ItemType directory | Out-Null

    Exec { msbuild $SolutionFile /t:Clean "/p:Configuration=$Configuration" /v:quiet } 
}

Task Generate-CommonAssemblyInfo -depends Clean {
    # Get commit hash
    If (Test-Command git) {
        # Get commit
        if (!$Commit) {
            $Commit = (git rev-parse HEAD)

            (git diff-files --quiet)
            if($LASTEXITCODE -ne 0) {
                $Commit = $Commit + "*"
            }

            Write-BuildMessage "Updated commit hash to $Commit"
        }
    }
    
    Generate-Assembly-Info -file $AssemblyInfo `
                           -buildDate ([System.DateTime]::UtcNow).ToString("yyyy-MM-ddTHH\:mm\:ss.fffffffzzz") `
                           -commit $Commit `
                           -company "Viktor Elofsson - viktorelofsson.se" `
                           -product "Hadouken" `
                           -title "Hadouken" `
                           -description "A headless BitTorrent client for Windows." `
                           -version $Version `
                           -buildVersion $BuildVersion
}

Task Compile -depends Generate-CommonAssemblyInfo {
    Exec { msbuild $SolutionFile /t:Build "/p:Configuration=$Configuration" /v:quiet }
}

Task Test -depends Compile {
    Exec { & $Tools_xUnit "./src/Tests/Hadouken.Common.Tests/bin/x86/$Configuration/Hadouken.Common.Tests.dll" /xml (Join-Path $Dir_Artifacts "xunit-results-common.xml") }
    Exec { & $Tools_xUnit "./src/Tests/Hadouken.Core.Tests/bin/x86/$Configuration/Hadouken.Core.Tests.dll" /xml (Join-Path $Dir_Artifacts "xunit-results-core.xml") }
    Exec { & $Tools_xUnit "./src/Tests/Hadouken.Extensions.AutoAdd.Tests/bin/x86/$Configuration/Hadouken.Extensions.AutoAdd.Tests.dll" /xml (Join-Path $Dir_Artifacts "xunit-results-ext-autoadd.xml") }
    Exec { & $Tools_xUnit "./src/Tests/Hadouken.Extensions.AutoMove.Tests/bin/x86/$Configuration/Hadouken.Extensions.AutoMove.Tests.dll" /xml (Join-Path $Dir_Artifacts "xunit-results-ext-automove.xml") }
    Exec { & $Tools_xUnit "./src/Tests/Hadouken.Extensions.HipChat.Tests/bin/x86/$Configuration/Hadouken.Extensions.HipChat.Tests.dll" /xml (Join-Path $Dir_Artifacts "xunit-results-ext-hipchat.xml") }
    Exec { & $Tools_xUnit "./src/Tests/Hadouken.Extensions.Kodi.Tests/bin/x86/$Configuration/Hadouken.Extensions.Kodi.Tests.dll" /xml (Join-Path $Dir_Artifacts "xunit-results-ext-kodi.xml") }

    if($env:APPVEYOR) {
        # upload results to AppVeyor
        $wc = New-Object "System.Net.WebClient"
        $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Join-Path $Dir_Artifacts "xunit-results-common.xml"))
        $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Join-Path $Dir_Artifacts "xunit-results-core.xml"))
        $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Join-Path $Dir_Artifacts "xunit-results-ext-autoadd.xml"))
        $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Join-Path $Dir_Artifacts "xunit-results-ext-automove.xml"))
        $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Join-Path $Dir_Artifacts "xunit-results-ext-hipchat.xml"))
        $wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Join-Path $Dir_Artifacts "xunit-results-ext-kodi.xml"))
    }
}

Task Output -depends Compile {
    $filter = ("*.dll", "*.exe")

    Write-BuildMessage "Copying files from $Dir_Output to $Dir_Binaries"

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

    # Copy tools
    $poshTool = ".\src\Tools\Hadouken.Tools.Posh\bin\x86\$Configuration\Hadouken.Tools.Posh.dll"
    $poshToolManifest = ".\src\Tools\Hadouken.Tools.Posh\bin\x86\$Configuration\Hadouken.Tools.Posh.psd1"
    $poshToolOut = Join-Path $Dir_Binaries "Tools/PowerShellModules/Hadouken.Tools.Posh"
    New-Item $poshToolOut -ItemType directory
    Copy-Item -Path $poshTool -Destination $poshToolOut
    Copy-Item -Path $poshToolManifest -Destination $poshToolOut

    # Zip the web UI
    Write-BuildMessage "Compressing and packaging the web ui"
    $webuiZip = Join-Path $Dir_Binaries "Web/web.zip"
    Exec {
        & $Tools_7za a -mx=9 $webuiZip .\src\Web\*
    }
}

Task Zip -depends Output {
    $files = "$Dir_Binaries/*"
    $output = Join-Path $Dir_Artifacts $Artifact_Zip

    # Copy the correct config file
    $configTarget = Join-Path $Dir_Binaries "Hadouken.exe.config"
    Copy-Item -Path ".\src\Configuration\$Configuration\Hadouken.exe.zip.config" -Destination $configTarget

    Exec {
        & $Tools_7za a -mx=9 $output $files
    }
}

Task MSI {
    $wxs = Get-ChildItem "src\Installer\" -Include *.wxs -recurse | Select-Object FullName | foreach {$_.FullName}

    # Copy the correct config file
    $configTarget = Join-Path $Dir_Binaries "Hadouken.exe.config"
    Copy-Item -Path ".\src\Configuration\$Configuration\Hadouken.exe.msi.config" -Destination $configTarget

    Exec {
        & $Tools_WixCandle "-dBinDir=$Dir_Binaries" "-dBuildVersion=$Version" -dConfigDir=src\Configuration\Service -ext WixUtilExtension -ext WixFirewallExtension -o "$Dir_Artifacts\wixobj\\" $wxs
    }

    $wixobj = Get-ChildItem "$Dir_Artifacts\wixobj\" -Include *.wixobj -Recurse | Select-Object FullName | foreach {$_.FullName}
    $msi = Join-Path $Dir_Artifacts $Artifact_Msi

    Exec {
        & $Tools_WixLight -ext WixUIExtension -ext WixUtilExtension -ext WixNetFxExtension -ext WixFirewallExtension -sval -o $msi -cultures:en-us -loc "src\Installer\Hadouken.en-us.wxl"  $wixobj
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
        & $Tools_NuGet pack "$Dir_Artifacts/choco/Hadouken.nuspec" -Version $BuildVersion -NoPackageAnalysis -OutputDirectory $Dir_Artifacts
    }
}

Task Publish-AppVeyor -depends MSI, Zip -precondition { return $env:APPVEYOR } {
    Write-BuildMessage "Publishing build artifacts to AppVeyor"

    Push-AppveyorArtifact (Join-Path $Dir_Artifacts $Artifact_Choco)
    Push-AppveyorArtifact (Join-Path $Dir_Artifacts $Artifact_Msi)
    Push-AppveyorArtifact (Join-Path $Dir_Artifacts $Artifact_Zip)
}

Task Publish-GitHub -depends MSI, Zip {
    $data = @{
        tag_name         = "v$Version"
        target_commitish = "master"
        name             = "$Name $Version"
    }

    Write-BuildMessage "Creating release $($data.tag_name)"

    $release = New-GitHubRelease "https://api.github.com/repos/$GitHub_Owner/$GitHub_Repository/releases" $data $GitHub_Token
    $releaseMSI = Join-Path $Dir_Artifacts $Artifact_Zip
    $releaseZip = Join-Path $Dir_Artifacts $Artifact_Msi

    Write-BuildMessage "Uploading $releaseMSI to github.com/$GitHub_Owner/$GitHub_Repository"
    Upload-GitHubReleaseAsset $GitHub_Token $GitHub_Owner $GitHub_Repository $release.Id $releaseMSI "application/octet-stream" | Out-Null

    Write-BuildMessage "Uploading $releaseZip to github.com/$GitHub_Owner/$GitHub_Repository"
    Upload-GitHubReleaseAsset $GitHub_Token $GitHub_Owner $GitHub_Repository $release.Id $releaseZip "application/zip" | Out-Null
}

Task Publish-Chocolatey -depends Chocolatey {
    Write-BuildMessage "Pushing Hadouken $BuildVersion to Chocolatey"

    $pkg = Join-Path $Dir_Artifacts $Artifact_Choco
    Exec {
        & $Tools_NuGet push $pkg $Chocolatey_API_Key -Source $Chocolatey_Source
    }
}
