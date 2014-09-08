param (
    [string]$ChocolateyAPIKey  = $env:CHOCOLATEY_API_KEY,
    [string]$GitHubToken       = $env:GITHUB_TOKEN,
    [string]$Task              = "Default",
    [string]$BuildNumber       = $null
)

$Version = Get-Content .\VERSION
$BuildVersion = $Version

if($BuildNumber) {
    $BuildVersion = "$Version-build-$BuildNumber"
}

$nuget = Join-Path $PSScriptRoot "tools/nuget.exe"
Start-Process -NoNewWindow -Wait $nuget "restore packages.config -PackagesDirectory packages"
Start-Process -NoNewWindow -Wait $nuget "restore Hadouken.sln    -PackagesDirectory packages"

$params = @{
    "Chocolatey_API_Key" = $ChocolateyAPIKey
    "GitHub_Token" = $GitHubToken
    "Version" = $Version
    "BuildVersion" = $BuildVersion
}

Import-Module .\packages\psake.4.3.2\tools\psake.psm1

$psake.use_exit_on_error = $true

Invoke-psake build.ps1 -framework '4.0' -parameters $params $Task

if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }