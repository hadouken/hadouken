@echo off

cd /d ../src/Shared
copy CommonAssemblyInfo.local.cs CommonAssemblyInfo.cs

cd /d ../../

%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Hadouken.sln /nologo /t:Rebuild /p:Configuration=Debug
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Hadouken.sln /nologo /t:Rebuild /p:Configuration=Release

mklink /J "src/Hosts/Hadouken.Hosts.CommandLine/bin/Debug/WebUI" "src/WebUI"
mklink /J "src/Hosts/Hadouken.Hosts.CommandLine/bin/Release/WebUI" "src/WebUI"

pause