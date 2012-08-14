param
(
    [string]$key,
    [string]$accessKey,
    [string]$bucket,
    [string]$file,
    [string]$subdir
)

$fileInfo = Get-ChildItem $file

Add-Type -Path "..\awssdk\bin\AWSSDK.dll"

$client=[Amazon.AWSClientFactory]::CreateAmazonS3Client($key, $accessKey)

$request = New-Object -TypeName Amazon.S3.Model.PutObjectRequest

[void] $request.WithFilePath($fileInfo.FullName)
[void] $request.WithBucketName($bucket)
[void] $request.WithKey($subdir + "/" + $fileInfo.Name)

[void] $client.PutObject($request)

if(Test-Path ($fileInfo.FullName + ".md5"))
{
    $request = New-Object -TypeName Amazon.S3.Model.PutObjectRequest
    
    [void] $request.WithFilePath($fileInfo.FullName + ".md5")
    [void] $request.WithBucketName($bucket)
    [void] $request.WithKey($subdir + "/" + $fileInfo.Name + ".md5")

    [void] $client.PutObject($request)
}