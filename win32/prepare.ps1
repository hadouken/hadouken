# This script will download the third-party libraries required to build
# Hadouken on Windows.

$NUGET_TOOL         = Join-Path (Convert-Path .) "win32/tools/nuget.exe"

& "$NUGET_TOOL" install win32/packages.config -OutputDirectory packages