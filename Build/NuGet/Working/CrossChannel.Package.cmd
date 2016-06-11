@echo off
echo Creating NuGet package

echo.
echo Cleaning convention based working directory...
rmdir build /s /q
rmdir content /s /q
rmdir lib /s /q
rmdir tools /s /q

echo.
echo Creating convention based working directory...
mkdir build
mkdir content
mkdir lib
mkdir tools

echo.
echo Copying lib for net35...
xcopy ..\..\..\Source\CrossChannel.Net35\bin\Release\* lib\net35\* /s /e /y

echo.
echo Copying lib for net40...
xcopy ..\..\..\Source\CrossChannel.Net40\bin\Release\* lib\net40\* /s /e /y

echo.
echo Copying lib for net45...
xcopy ..\..\..\Source\CrossChannel.Net45\bin\Release\* lib\net45\* /s /e /y

echo.
echo Copying lib for net46...
xcopy ..\..\..\Source\CrossChannel.Net46\bin\Release\* lib\net46\* /s /e /y

echo.
echo Copying lib for net461...
xcopy ..\..\..\Source\CrossChannel.Net461\bin\Release\* lib\net461\* /s /e /y

echo.
echo Packaging...
..\..\..\Tools\NuGet\nuget.exe pack CrossChannel.nuspec

echo.
echo Moving package...
move CrossChannel.1.0.4.nupkg ..\Packages\

echo.
echo Pushing package...
..\..\..\Tools\NuGet\nuget.exe push ..\Packages\CrossChannel.1.0.4.nupkg 0db11efd-f306-4219-8b43-c314cd79385e -Source https://www.nuget.org/api/v2/package

pause