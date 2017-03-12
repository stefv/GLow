@echo off
cls

rem Check if the VS command prompt has been started
if ("%VSINSTALLDIR%"=="") goto NOTSET
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
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" GLow.iss /DArch=x86 /DInstallArch=

rem Build the x64 install
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" GLow.iss /DArch=x64 /DInstallArch=x64

exit /b 0

:NOTSET
echo The VSINSTALLDIR environment variable is not set.
echo Did you start the Visual Studio command prompt ?

exit /b 1