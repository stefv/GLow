; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "GLow"
#define MyAppVersion "1.0"
#define MyAppPublisher "St�phane VANPOPERYNGHE"
#define MyAppURL "https://github.com/stefv/GLow"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{BBF9A906-AA20-412F-8BD8-AB244C35BE41}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=C:\Users\svanp\Documents\GitHubVisualStudio\GLow\LICENSE
OutputDir=C:\Users\svanp\Documents\GitHubVisualStudio\GLow\Inno setup
OutputBaseFilename=GLow setup-{#MyAppVersion}
SetupIconFile=C:\Users\svanp\Documents\GitHubVisualStudio\GLow\Images\glow.ico
Compression=lzma
SolidCompression=yes


;------------------------------------------------------------------------------
; Open the screensaver Properties dialog with newly installed screensaver
; selected.
;------------------------------------------------------------------------------

Filename: "{sys}\rundll32.exe"; Parameters: "desk.cpl,InstallScreenSaver {app}\GLow.scr"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Files]
Source: "..\bin\Release\GLow.scr"; DestDir: "{app}"
Source: "..\bin\Release\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
Source: "..\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "..\bin\Release\OpenTK.dll"; DestDir: "{app}"
Source: "..\bin\Release\OpenTK.GLControl.dll"; DestDir: "{app}"
Source: "..\Lib\x86\sqlite3.dll"; DestDir: "{app}"

[Tasks]
Name: "CheckDotNet"; Description: "Check installation .NET (4.0 client)"; Check: InitializeSetup

[UninstallDelete]
;Type: files; Name: "{userappdata}\Roaming\GLow_Screensaver\GLow Screensaver\GLow.sqlite"

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key: string;
    install, serviceCount: cardinal;
    success: boolean;
begin
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;
    // .NET 4.0 uses value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;
    result := success and (install = 1) and (serviceCount >= service);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4\Client', 0) then begin
        MsgBox('MyApp requires Microsoft .NET Framework 4.0 Client Profile.'#13#13
            'Please use Windows Update to install this version,'#13
            'and then re-run the MyApp setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;