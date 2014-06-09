param (
    [string]$ChocolateyAPIKey  = $null,
    [string]$GitHubToken       = $null,
    [string]$NuGetAPIKey       = $null,
    [string]$Task              = "Default",
    [string]$Version           = "0.0.0"
 )

$nuget = Join-Path $PSScriptRoot "tools/nuget.exe"
Start-Process -NoNewWindow -Wait $nuget "restore packages.config -PackagesDirectory packages"
Start-Process -NoNewWindow -Wait $nuget "restore Hadouken.sln    -PackagesDirectory packages"

$params = @{
    "Chocolatey_API_Key" = $ChocolateyAPIKey
    "GitHub_Token" = $GitHubToken
    "NuGet_API_Key" = $NuGetAPIKey
    "Version" = $Version
}

Import-Module .\packages\psake.4.3.2\tools\psake.psm1
Invoke-psake build.ps1 -framework '4.0' -parameters $params $Task