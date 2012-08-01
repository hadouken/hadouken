@ECHO OFF

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe Hadouken.msbuild /t:Build

@PAUSE