param (
    [string]$Version = "0.0.0"
 )

$nuget = Join-Path $PSScriptRoot "tools/nuget.exe"
Start-Process -NoNewWindow -Wait $nuget "restore packages.config -PackagesDirectory packages"
Start-Process -NoNewWindow -Wait $nuget "restore Hadouken.sln    -PackagesDirectory packages"

Import-Module .\packages\psake.4.3.2\tools\psake.psm1
Invoke-psake build.ps1 -framework '4.0' -parameters @{"Version"=$Version}