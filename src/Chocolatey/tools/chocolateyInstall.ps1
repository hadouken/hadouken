$packageName = 'hadouken'
$installerType = 'msi' 
$url = '%DOWNLOAD_URL%'
$silentArgs = '/quiet'
$validExitCodes = @(0,3010)

Install-ChocolateyPackage "$packageName" "$installerType" "$silentArgs" "$url" -validExitCodes $validExitCodes