function Test-Command
{
param(
    $Command
)
     try {if(Get-Command $command){RETURN $true}}
     catch {RETURN $false}
}

function Write-BuildMessage() {
    param($message)

    Write-Host $message

    if($env:APPVEYOR) {
        Add-AppveyorMessage "$message"
    }
}

function Generate-Assembly-Info
{
param(
	[string]$clsCompliant = "true",
	[string]$title, 
	[string]$description, 
	[string]$company, 
	[string]$product, 
	[string]$copyright, 
	[string]$version,
    [string]$buildVersion,
    [string]$commit,
    [string]$buildDate,
	[string]$file = $(throw "file is a required parameter.")
)
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//[assembly: CLSCompliantAttribute($clsCompliant )]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$buildVersion / $commit"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]

namespace Hadouken
{
    public static class AssemblyInformation
    {
        public static readonly string Commit = ""$commit"";
        public static readonly DateTime BuildDate = DateTime.Parse(""$buildDate"");
    }
}

"

	$dir = [System.IO.Path]::GetDirectoryName($file)
	if ([System.IO.Directory]::Exists($dir) -eq $false)
	{
		Write-Host "Creating directory $dir"
		[System.IO.Directory]::CreateDirectory($dir)
	}
	Write-BuildMessage "Generating assembly info file: $file"
	Write-Output $asmInfo > $file
}