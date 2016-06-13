@echo off
echo Creating NuGet package
set Name=CrossChannel
set Version=%APPVEYOR_BUILD_VERSION%
cd /d %~dp0

echo.
echo Cleaning convention based working directory...
rmdir Working\build /s /q
rmdir Working\content /s /q
rmdir Working\lib /s /q
rmdir Working\tools /s /q

echo.
echo Creating convention based working directory...
mkdir Working\build
mkdir Working\content
mkdir Working\lib
mkdir Working\tools

echo.
echo Copying lib for net35...
xcopy ..\..\Source\%Name%.Net35\bin\Release\* Working\lib\net35\* /s /e /y

echo.
echo Copying lib for net40...
xcopy ..\..\Source\%Name%.Net40\bin\Release\* Working\lib\net40\* /s /e /y

echo.
echo Copying lib for net45...
xcopy ..\..\Source\%Name%.Net45\bin\Release\* Working\lib\net45\* /s /e /y

echo.
echo Copying lib for net46...
xcopy ..\..\Source\%Name%.Net46\bin\Release\* Working\lib\net46\* /s /e /y

echo.
echo Copying lib for net461...
xcopy ..\..\Source\%Name%.Net461\bin\Release\* Working\lib\net461\* /s /e /y

echo.
echo Packaging...
..\..\Tools\NuGet\nuget.exe pack Working\%Name%.nuspec -Version %Version%

echo.
echo Moving package...
move %Name%.%Version%.nupkg Packages

echo.
echo Pushing package...
..\..\Tools\NuGet\nuget.exe push Packages\%Name%.%Version%.nupkg %NuGetApiKey% -Source https://www.nuget.org/api/v2/package
