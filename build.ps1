Properties {
    $Root = Split-Path $psake.build_script_file

    $BuildArtifactsDirectory = Join-Path $Root "build"
    $BinariesDirectory = Join-Path $BuildArtifactsDirectory "bin"
    $Configuration = "Release"
    $Version = "0.0.0"

    $7za       = Join-Path $Root "tools\7za-9.20\7za.exe"
    $NuGet     = Join-Path $Root "tools\nuget.exe"
    $WixCandle = Join-Path $Root "packages\WiX.Toolset.3.8.1128.0\tools\wix\candle.exe"
    $WixLight  = Join-Path $Root "packages\WiX.Toolset.3.8.1128.0\tools\wix\light.exe"
    $xUnit     = Join-Path $Root "packages\xunit.runners.1.9.2\tools\xunit.console.clr4.x86.exe"
}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -depends Clean, Compile, Test, Output, Zip, MSI, NuGet

Task Clean {
    Write-Host "Cleaning and creating build artifacts folder."

    if (Test-Path $BuildArtifactsDirectory)
    {
        Remove-Item $BuildArtifactsDirectory -Recurse -Force | Out-Null
    }

    New-Item $BuildArtifactsDirectory -ItemType directory | Out-Null
    New-Item $BinariesDirectory -ItemType directory | Out-Null

    Exec { msbuild "Hadouken.sln" /t:Clean "/p:Configuration=$Configuration" /v:quiet } 
}

Task Compile -depends Clean {
    Exec { msbuild "Hadouken.sln" /t:Build "/p:Configuration=$Configuration" /v:quiet }
}

Task Test -depends Compile {
    Exec { & $xUnit ".\src\Main\Hadouken.Tests\bin\$Configuration\Hadouken.Tests.dll" }
}

Task Output -depends Compile {
    $source = Join-Path $Root ".\src\Main\Hadouken.Service\bin\$Configuration\*"
    $filter = ("*.dll", "*.exe")

    # Copy files
    Get-ChildItem $source -Include $filter | %{Copy-Item -Path $_.FullName -Destination $BinariesDirectory}

    # Copy the correct config file
    Copy-Item -Path .\src\Configuration\Console\Hadouken.Service.exe.config -Destination $BinariesDirectory
}

Task Zip -depends Output {
    $files = "$BinariesDirectory/*.*"
    $output = Join-Path $BuildArtifactsDirectory "hdkn-$Version.zip"

    Exec {
        & $7za a -mx=9 $output $files
    }
}

Task MSI -depends Output {
    $wxs = Get-ChildItem "src\Installer\" -Include *.wxs -recurse | Select-Object FullName | foreach {$_.FullName}

    Exec {
        & $WixCandle "-dBinDir=$BinariesDirectory" "-dBuildVersion=$Version" -dConfigDir=src\Configuration\Service -ext WixUtilExtension -o "$BuildArtifactsDirectory\wixobj\" $wxs
    }

    $wixobj = Get-ChildItem "$BuildArtifactsDirectory\wixobj\" -Include *.wixobj -Recurse | Select-Object FullName | foreach {$_.FullName}
    $msi = "$BuildArtifactsDirectory\hdkn-$Version.msi"

    Exec {
        & $WixLight -ext WixUIExtension -ext WixUtilExtension -ext WixNetFxExtension -o $msi $wixobj
    }
}

Task NuGet -depends Output {
    Exec {
        & $NuGet pack Hadouken.SDK.nuspec -Version $Version -NoPackageAnalysis -OutputDirectory $BuildArtifactsDirectory
    }
}