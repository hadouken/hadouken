# Import scripts
. .\build.github.ps1

$Root = $PSScriptRoot

Properties {
    $Dir = @{
        Artifacts = Join-Path $Root "build"
        Binaries  = Join-Path $Root "build/bin"
    }

    $Configuration = "Release"
    $DeployToken   = ""
    $SolutionFile  = "Hadouken.sln"

    $Tools = @{
        '7za'     = Join-Path $Root "tools\7za-9.20\7za.exe"
        NuGet     = Join-Path $Root "tools\nuget.exe"
        WixCandle = Join-Path $Root "packages\WiX.Toolset.3.8.1128.0\tools\wix\candle.exe"
        WixLight  = Join-Path $Root "packages\WiX.Toolset.3.8.1128.0\tools\wix\light.exe"
        xUnit     = Join-Path $Root "packages\xunit.runners.1.9.2\tools\xunit.console.clr4.x86.exe"
    }

    $Version = "0.0.0"
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -depends Clean, Compile, Test, Output, Zip, MSI, NuGet

Task Clean {
    Write-Host "Cleaning and creating build artifacts folder."

    if (Test-Path $Dir.Artifacts)
    {
        Remove-Item $Dir.Artifacts -Recurse -Force | Out-Null
    }

    New-Item $Dir.Artifacts -ItemType directory | Out-Null
    New-Item $Dir.Binaries  -ItemType directory | Out-Null

    Exec { msbuild $SolutionFile /t:Clean "/p:Configuration=$Configuration" /v:quiet } 
}

Task Compile -depends Clean {
    Exec { msbuild $SolutionFile /t:Build "/p:Configuration=$Configuration" /v:quiet }
}

Task Test -depends Compile {
    Exec { & $Tools.xUnit ".\src\Main\Hadouken.Tests\bin\$Configuration\Hadouken.Tests.dll" }
}

Task Output -depends Compile {
    $source = Join-Path $Root "src\Main\Hadouken.Service\bin\$Configuration\*"
    $filter = ("*.dll", "*.exe")

    Write-Host "Copying files from $source to $($Dir.Binaries)"

    # Copy files
    Get-ChildItem $source -Include $filter | %{Copy-Item -Path $_.FullName -Destination $Dir.Binaries}

    # Copy the correct config file
    Copy-Item -Path .\src\Configuration\Console\Hadouken.Service.exe.config -Destination $Dir.Binaries
}

Task Zip -depends Output {
    $files = "$($Dir.Binaries)/*.*"
    $output = Join-Path $Dir.Artifacts "hdkn-$Version.zip"

    Exec {
        & $Tools.'7za' a -mx=9 $output $files
    }
}

Task MSI -depends Output {
    Write-Host "Installing WiX 3.8.1128"

    Exec {
        & $Tools.NuGet install WiX.Toolset -Version 3.8.1128 -OutputDirectory packages
    }

    $wxs = Get-ChildItem "src\Installer\" -Include *.wxs -recurse | Select-Object FullName | foreach {$_.FullName}

    Exec {
        & $Tools.WixCandle "-dBinDir=$($Dir.Binaries)" "-dBuildVersion=$Version" -dConfigDir=src\Configuration\Service -ext WixUtilExtension -o "$($Dir.Artifacts)\wixobj\" $wxs
    }

    $wixobj = Get-ChildItem "$($Dir.Artifacts)\wixobj\" -Include *.wixobj -Recurse | Select-Object FullName | foreach {$_.FullName}
    $msi = "$($Dir.Artifacts)\hdkn-$Version.msi"

    Exec {
        & $Tools.WixLight -ext WixUIExtension -ext WixUtilExtension -ext WixNetFxExtension -o $msi $wixobj
    }
}

Task NuGet -depends Output {
    Exec {
        & $Tools.NuGet pack Hadouken.SDK.nuspec -Version $Version -NoPackageAnalysis -OutputDirectory $Dir.Artifacts
    }
}

Task Publish -depends MSI, Zip, NuGet {
    $data = @{
        tag_name         = "v$Version"
        target_commitish = "master"
        name             = "Hadouken $Version"
    }

    New-GitHubRelease "https://api.github.com/repos/hadouken/hadouken/releases" $data $DeployToken
}