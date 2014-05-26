Add-Type -AssemblyName System.Net.Http

function New-GitHubRelease() {
    param(
        [string] $target,
        [object] $data,
	    [string] $token
    )

    $netAssembly = [Reflection.Assembly]::GetAssembly([System.Net.Configuration.SettingsSection])

    if($netAssembly)
    {
        $bindingFlags = [Reflection.BindingFlags] "Static,GetProperty,NonPublic"
        $settingsType = $netAssembly.GetType("System.Net.Configuration.SettingsSectionInternal")

        $instance = $settingsType.InvokeMember("Section", $bindingFlags, $null, $null, @())

        if($instance)
        {
            $bindingFlags = "NonPublic","Instance"
            $useUnsafeHeaderParsingField = $settingsType.GetField("useUnsafeHeaderParsing", $bindingFlags)

            if($useUnsafeHeaderParsingField)
            {
              $useUnsafeHeaderParsingField.SetValue($instance, $true)
            }
        }
    }

    $client = New-Object System.Net.Http.HttpClient
    $client.DefaultRequestHeaders.Authorization = New-Object System.Net.Http.Headers.AuthenticationHeaderValue("token", $token)
    $client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Hadouken Deployment Script");

    $json = ConvertTo-Json -InputObject $data
    $request = New-Object System.Net.Http.StringContent($json)
    
    $result = $client.PostAsync($target, $request).Result;
    $resultData = $result.Content.ReadAsStringAsync().Result;

    return ConvertFrom-Json -InputObject $resultData;
}

function Upload-GitHubReleaseAsset() {
    param(
        [string] $token, 
        [string] $owner,
        [string] $repo,
        [int]    $releaseId,
        [string] $inputFile,
        [string] $contentType
    )

    $fileName = [System.IO.Path]::GetFileName($inputFile)
    $url = "https://uploads.github.com/repos/$owner/$repo/releases/$releaseId/assets?name=$fileName"

    $client = New-Object System.Net.Http.HttpClient
    $client.DefaultRequestHeaders.Authorization = New-Object System.Net.Http.Headers.AuthenticationHeaderValue("token", $token)
    $client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Hadouken Deployment Script");

    $fileData = [System.IO.File]::ReadAllBytes($inputFile)
    $content = New-Object System.Net.Http.ByteArrayContent(,$fileData)
    $content.Headers.ContentType = New-Object System.Net.Http.Headers.MediaTypeHeaderValue($contentType)

    $result = $client.PostAsync($url, $content).Result;
    $resultData = $result.Content.ReadAsStringAsync().Result;

    return ConvertFrom-Json -InputObject $resultData;
}