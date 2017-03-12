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
LicenseFile=..\LICENSE
OutputDir=.\
OutputBaseFilename=GLow Setup-{#MyAppVersion}.{#Arch}
SetupIconFile=.\glow.ico
Compression=lzma
SolidCompression=yes
; "ArchitecturesInstallIn64BitMode=x64" requests that the install be
; done in "64-bit mode" on x64, meaning it should use the native
; 64-bit Program Files directory and the 64-bit view of the registry.
; On all other architectures it will install in "32-bit mode".
ArchitecturesInstallIn64BitMode={#InstallArch}
; Note: We don't set ProcessorsAllowed because we want this
; installation to run on all architectures (including Itanium,
; since it's capable of running 32-bit code too).
[Run]

;------------------------------------------------------------------------------
; Open the screensaver Properties dialog with newly installed screensaver
; selected.
;------------------------------------------------------------------------------

;Filename: "{sys}\rundll32.exe"; Parameters: "desk.cpl,InstallScreenSaver {app}\GLow.scr"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Files]
;Source: "..\GLow Screensaver\bin\Debug\GLow.scr"; DestDir: "{app}"
;Source: "..\GLow Screensaver\bin\Release\ICSharpCode.AvalonEdit.dll"; DestDir: "{app}"
;Source: "..\GLow Screensaver\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
;Source: "..\GLow Screensaver\bin\Release\OpenTK.dll"; DestDir: "{app}"
;Source: "..\GLow Screensaver\bin\Release\OpenTK.GLControl.dll"; DestDir: "{app}"
;Source: "..\GLow Screensaver\Lib\x86\sqlite3.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\GLowService.exe"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\sqlite3.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\EntityFramework.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\EntityFramework.SqlServer.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\GLowCommon.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\Newtonsoft.Json.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\System.Data.SQLite.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\System.Data.SQLite.EF6.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\System.Data.SQLite.Linq.dll"; DestDir: "{app}"
Source: ".\bin\{#Arch}\Release\{#Arch}\SQLite.Interop.dll"; DestDir: "{app}"

[Tasks]
Name: "CheckDotNet"; Description: "Check installation .NET (4.5.2)"; Check: InitializeSetup

[UninstallDelete]
;Type: files; Name: "{userappdata}\Roaming\GLow_Screensaver\GLow Screensaver\GLow.sqlite"

[Code]
function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1'          .NET Framework 1.1
//    'v2.0'          .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//    'v4.5'          .NET Framework 4.5
//    'v4.5.1'        .NET Framework 4.5.1
//    'v4.5.2'        .NET Framework 4.5.2
//    'v4.6'          .NET Framework 4.6
//    'v4.6.1'        .NET Framework 4.6.1
//    'v4.6.2'        .NET Framework 4.6.2
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
    key, versionKey: string;
    install, release, serviceCount, versionRelease: cardinal;
    success: boolean;
begin
    versionKey := version;
    versionRelease := 0;

    // .NET 1.1 and 2.0 embed release number in version key
    if version = 'v1.1' then begin
        versionKey := 'v1.1.4322';
    end else if version = 'v2.0' then begin
        versionKey := 'v2.0.50727';
    end

    // .NET 4.5 and newer install as update to .NET 4.0 Full
    else if Pos('v4.', version) = 1 then begin
        versionKey := 'v4\Full';
        case version of
          'v4.5':   versionRelease := 378389;
          'v4.5.1': versionRelease := 378675; // 378758 on Windows 8 and older
          'v4.5.2': versionRelease := 379893;
          'v4.6':   versionRelease := 393295; // 393297 on Windows 8.1 and older
          'v4.6.1': versionRelease := 394254; // 394271 on Windows 8.1 and older
          'v4.6.2': versionRelease := 394802; // 394806 on Windows 8.1 and older
        end;
    end;

    // installation key group for all .NET versions
    key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + versionKey;

    // .NET 3.0 uses value InstallSuccess in subkey Setup
    if Pos('v3.0', version) = 1 then begin
        success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
    end else begin
        success := RegQueryDWordValue(HKLM, key, 'Install', install);
    end;

    // .NET 4.0 and newer use value Servicing instead of SP
    if Pos('v4', version) = 1 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
    end else begin
        success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
    end;

    // .NET 4.5 and newer use additional value Release
    if versionRelease > 0 then begin
        success := success and RegQueryDWordValue(HKLM, key, 'Release', release);
        success := success and (release >= versionRelease);
    end;

    result := success and (install = 1) and (serviceCount >= service);
end;

function InitializeSetup(): Boolean;
begin
    if not IsDotNetDetected('v4.5.2', 0) then begin
        MsgBox('GLow requires Microsoft .NET Framework 4.5.2.'#13#13
            'Please use Windows Update to install this version,'#13
            'and then re-run the GLow setup program.', mbInformation, MB_OK);
        result := false;
    end else
        result := true;
end;
