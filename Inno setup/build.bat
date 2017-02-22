cls

rem Build the x86 version
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" "..\GLow Screensaver.sln" /t:Rebuild /p:Configuration=Release /p:Platform="x86"
if ERRORLEVEL 1 exit /b 1

rem Build the x64 version
"%ProgramFiles(x86)%\MSBuild\14.0\Bin\MsBuild.exe" "..\GLow Screensaver.sln" /t:Rebuild /p:Configuration=Release /p:Platform="x64"
if ERRORLEVEL 1 exit /b 1

rem Build the x86 install
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" GLow.iss

exit /b 0