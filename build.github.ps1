Add-Type -AssemblyName System.Net.Http

<#
.SYNOPSIS
Performs synchronous HTTP communication with the target specified.
.DESCRIPTION
Using the System.Net.WebRequest object, creates an HTTP web request 
populating it with the $content specified and submitting the request 
to the $target indicated using the incoming $verb.
.PARAMETER target
The target URL to communicate with
.PARAMETER authHeader
The content of an AUTHORIZATION header to add to the HTTP request.
Examples: "Basic someEncodedUserPass" or "token someOAuthToken"
.PARAMETER verb
The HTTP verb to assign the request.
.PARAMETER content
The string content to encode and add to the request body.
#>
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
}