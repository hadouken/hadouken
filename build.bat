@echo off

:Build
cls

if not exist tools\FAKE.Core\tools\Fake.exe ( 
	echo Installing FAKE...
	"tools\nuget\nuget.exe" "install" "FAKE.Core" "-OutputDirectory" "tools" "-ExcludeVersion" "-Prerelease"
	echo.
)

SET TARGET="All"
IF NOT [%1]==[] (set TARGET="%1")
SET BUILDMODE="Release"
IF NOT [%2]==[] (set BUILDMODE="%2")

echo Starting FAKE...
"tools\FAKE.Core\tools\Fake.exe" "build.fsx" "target=%TARGET%" "buildMode=%BUILDMODE%"

rem Loop the build script.
echo.
set CHOICE=nothing
echo (R)ebuild or (Enter) to exit
set /P CHOICE= 
if /i "%CHOICE%"=="R" goto :Build

:Quit
exit /b %errorlevel%