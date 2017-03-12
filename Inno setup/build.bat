@echo off
cls

rem Check if the version number is given
if (%1) == () goto VersionMissing

rem Check if the VS command prompt has been started
if ("%VSINSTALLDIR%"=="") goto NotSet
set MSBUILD=%VSINSTALLDIR%\MSBuild\%VisualStudioVersion%\Bin

rem Remove the previous EXE
del glow*.exe

rem Build the x86 version
"%MSBUILD%\MsBuild.exe" "..\GLow Screensaver.sln" /t:Rebuild /p:Configuration=Release /p:Platform="x86"
if ERRORLEVEL 1 exit /b 1

rem Build the x64 version
"%MSBUILD%\MsBuild.exe" "..\GLow Screensaver.sln" /t:Rebuild /p:Configuration=Release /p:Platform="x64"
if ERRORLEVEL 1 exit /b 1

rem Build the x86 install
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" GLow.iss /DArch=x86 /DInstallArch= /DVersion=%1

rem Build the x64 install
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" GLow.iss /DArch=x64 /DInstallArch=x64 /DVersion=%1

exit /b 0

:NotSet
echo The VSINSTALLDIR environment variable is not set.
echo Did you start the Visual Studio command prompt ?

exit /b 1

:VersionMissing
echo The version number is missing.

exit /b 1
