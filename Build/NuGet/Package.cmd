@echo off
echo Creating NuGet package...

cd /d %~dp0

mkdir Working
mkdir Working\build
mkdir Working\content
mkdir Working\lib
mkdir Working\tools

copy Package.nuspec Working\%NUGET_PACKAGE_ID%.nuspec
xcopy ..\..\Source\%APPVEYOR_PROJECT_NAME%.Net35\bin\Release\* Working\lib\net35\* /s /e /y
xcopy ..\..\Source\%APPVEYOR_PROJECT_NAME%.Net40\bin\Release\* Working\lib\net40\* /s /e /y
xcopy ..\..\Source\%APPVEYOR_PROJECT_NAME%.Net45\bin\Release\* Working\lib\net45\* /s /e /y
xcopy ..\..\Source\%APPVEYOR_PROJECT_NAME%.Net46\bin\Release\* Working\lib\net46\* /s /e /y
xcopy ..\..\Source\%APPVEYOR_PROJECT_NAME%.Net461\bin\Release\* Working\lib\net461\* /s /e /y

nuget.exe pack Working\%NUGET_PACKAGE_ID%.nuspec -Version %APPVEYOR_BUILD_VERSION%