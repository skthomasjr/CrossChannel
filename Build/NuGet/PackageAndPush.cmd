@echo off
echo Creating NuGet package
set Name=%APPVEYOR_PROJECT_NAME%
set Version=%APPVEYOR_BUILD_VERSION%
cd /d %~dp0

echo.
echo Creating convention based working directory...
mkdir Working\build
mkdir Working\content
mkdir Working\lib
mkdir Working\tools

echo.
echo Copying to lib...
xcopy ..\..\Source\%Name%.Net35\bin\Release\* Working\lib\net35\* /s /e /y
xcopy ..\..\Source\%Name%.Net40\bin\Release\* Working\lib\net40\* /s /e /y
xcopy ..\..\Source\%Name%.Net45\bin\Release\* Working\lib\net45\* /s /e /y
xcopy ..\..\Source\%Name%.Net46\bin\Release\* Working\lib\net46\* /s /e /y
xcopy ..\..\Source\%Name%.Net461\bin\Release\* Working\lib\net461\* /s /e /y

echo.
echo Packaging...
..\..\Tools\NuGet\nuget.exe pack Working\%NuGetPackageId%.nuspec -Version %Version%

echo.
echo Pushing package...
..\..\Tools\NuGet\nuget.exe push %NuGetPackageId%.%Version%.nupkg %NuGetApiKey% -Source https://www.nuget.org/api/v2/package