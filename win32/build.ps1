Param(
    [string]$Script = "build.cake",
    [string]$Target = "Default",
    [string]$Configuration = "Debug",
    [string]$Verbosity = "Verbose"
)

function Get-ScriptDirectory {
    Split-Path -parent $PSCommandPath
}

$TOOLS_DIR = Join-Path (Get-ScriptDirectory) "tools"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"
$CAKE_EXE = Join-Path $TOOLS_DIR "Cake/Cake.exe"

# Make sure NuGet exists where we expect it.
if (!(Test-Path $NUGET_EXE)) {
    Throw "Could not find NuGet.exe at $NUGET_EXE"
}

# Restore Cake from NuGet.
& $NUGET_EXE install Cake -OutputDirectory $TOOLS_DIR -ExcludeVersion

# Make sure that Cake has been installed.
if (!(Test-Path $CAKE_EXE)) {
    Throw "Could not find Cake.exe"
}

# Start Cake
Push-Location (Get-ScriptDirectory)
& $CAKE_EXE $Script -target="$Target" -configuration="$Configuration" -verbosity="$verbosity"
Pop-Location

exit $LastExitCode
