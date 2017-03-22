; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "GLow"
#define MyAppPublisher "St�phane VANPOPERYNGHE"
#define MyAppURL "https://github.com/stefv/GLow"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{BBF9A906-AA20-412F-8BD8-AB244C35BE41}
AppName={#MyAppName}
AppVersion={#Version}
;AppVerName={#MyAppName} {#Version}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
OutputDir=.\
OutputBaseFilename=GLow Setup-{#Version}-{#Arch}
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
Filename: {sys}\sc.exe; Parameters: "create ""Glow"" start=auto binPath=""{app}\GLowService.exe""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "description ""Glow"" ""{cm:ServiceDescription}""" ; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop ""Glow""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete ""Glow""" ; Flags: runhidden
;------------------------------------------------------------------------------
; Open the screensaver Properties dialog with newly installed screensaver
; selected.
;------------------------------------------------------------------------------

;Filename: "{sys}\rundll32.exe"; Parameters: "desk.cpl,InstallScreenSaver {app}\GLow.scr"

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[CustomMessages]
ServiceDescription=Service to download shaders for GLow screensaver.
french.ServiceDescription=Service pour t�l�charger des shaders pour GLow screensaver.
CheckDotNet=Check installation .NET
french.CheckDotNet=V�rifier l'installation de .NET

[Icons]
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

[Files]
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
Name: "CheckDotNet"; Description: "{cm:CheckDotNet} (4.5.2)"; Check: InitializeSetup

[UninstallDelete]
;Type: files; Name: "{userappdata}\Roaming\GLow_Screensaver\GLow Screensaver\GLow.sqlite"

[Code]
type
	SERVICE_STATUS = record
    	dwServiceType				: cardinal;
    	dwCurrentState				: cardinal;
    	dwControlsAccepted			: cardinal;
    	dwWin32ExitCode				: cardinal;
    	dwServiceSpecificExitCode	: cardinal;
    	dwCheckPoint				: cardinal;
    	dwWaitHint					: cardinal;
	end;
	HANDLE = cardinal;

const
	SERVICE_QUERY_CONFIG		= $1;
	SERVICE_CHANGE_CONFIG		= $2;
	SERVICE_QUERY_STATUS		= $4;
	SERVICE_START				= $10;
	SERVICE_STOP				= $20;
	SERVICE_ALL_ACCESS			= $f01ff;
	SC_MANAGER_ALL_ACCESS		= $f003f;
	SERVICE_WIN32_OWN_PROCESS	= $10;
	SERVICE_WIN32_SHARE_PROCESS	= $20;
	SERVICE_WIN32				= $30;
	SERVICE_INTERACTIVE_PROCESS = $100;
	SERVICE_BOOT_START          = $0;
	SERVICE_SYSTEM_START        = $1;
	SERVICE_AUTO_START          = $2;
	SERVICE_DEMAND_START        = $3;
	SERVICE_DISABLED            = $4;
	SERVICE_DELETE              = $10000;
	SERVICE_CONTROL_STOP		= $1;
	SERVICE_CONTROL_PAUSE		= $2;
	SERVICE_CONTROL_CONTINUE	= $3;
	SERVICE_CONTROL_INTERROGATE = $4;
	SERVICE_STOPPED				= $1;
	SERVICE_START_PENDING       = $2;
	SERVICE_STOP_PENDING        = $3;
	SERVICE_RUNNING             = $4;
	SERVICE_CONTINUE_PENDING    = $5;
	SERVICE_PAUSE_PENDING       = $6;
	SERVICE_PAUSED              = $7;

// #######################################################################################
// nt based service utilities
// #######################################################################################
function OpenSCManager(lpMachineName, lpDatabaseName: string; dwDesiredAccess :cardinal): HANDLE;
external 'OpenSCManagerA@advapi32.dll stdcall';

function OpenService(hSCManager :HANDLE;lpServiceName: string; dwDesiredAccess :cardinal): HANDLE;
external 'OpenServiceA@advapi32.dll stdcall';

function CloseServiceHandle(hSCObject :HANDLE): boolean;
external 'CloseServiceHandle@advapi32.dll stdcall';

function CreateService(hSCManager :HANDLE;lpServiceName, lpDisplayName: string;dwDesiredAccess,dwServiceType,dwStartType,dwErrorControl: cardinal;lpBinaryPathName,lpLoadOrderGroup: String; lpdwTagId : cardinal;lpDependencies,lpServiceStartName,lpPassword :string): cardinal;
external 'CreateServiceA@advapi32.dll stdcall';

function DeleteService(hService :HANDLE): boolean;
external 'DeleteService@advapi32.dll stdcall';

function StartNTService(hService :HANDLE;dwNumServiceArgs : cardinal;lpServiceArgVectors : cardinal) : boolean;
external 'StartServiceA@advapi32.dll stdcall';

function ControlService(hService :HANDLE; dwControl :cardinal;var ServiceStatus :SERVICE_STATUS) : boolean;
external 'ControlService@advapi32.dll stdcall';

function QueryServiceStatus(hService :HANDLE;var ServiceStatus :SERVICE_STATUS) : boolean;
external 'QueryServiceStatus@advapi32.dll stdcall';

function QueryServiceStatusEx(hService :HANDLE;ServiceStatus :SERVICE_STATUS) : boolean;
external 'QueryServiceStatus@advapi32.dll stdcall';

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

function OpenServiceManager() : HANDLE;
begin
	if UsingWinNT() = true then begin
		Result := OpenSCManager('','ServicesActive',SC_MANAGER_ALL_ACCESS);
		if Result = 0 then
			MsgBox('the servicemanager is not available', mbError, MB_OK)
	end
	else begin
			MsgBox('only nt based systems support services', mbError, MB_OK)
			Result := 0;
	end
end;

function IsServiceInstalled(ServiceName: string) : boolean;
var
	hSCM	: HANDLE;
	hService: HANDLE;
begin
	hSCM := OpenServiceManager();
	Result := false;
	if hSCM <> 0 then begin
		hService := OpenService(hSCM,ServiceName,SERVICE_QUERY_CONFIG);
        if hService <> 0 then begin
            Result := true;
            CloseServiceHandle(hService)
		end;
        CloseServiceHandle(hSCM)
	end
end;

function InstallService(FileName, ServiceName, DisplayName, Description : string;ServiceType,StartType :cardinal) : boolean;
var
	hSCM	: HANDLE;
	hService: HANDLE;
begin
	hSCM := OpenServiceManager();
	Result := false;
	if hSCM <> 0 then begin
		hService := CreateService(hSCM,ServiceName,DisplayName,SERVICE_ALL_ACCESS,ServiceType,StartType,0,FileName,'',0,'','','');
		if hService <> 0 then begin
			Result := true;
			// Win2K & WinXP supports aditional description text for services
			if Description<> '' then
				RegWriteStringValue(HKLM,'System\CurrentControlSet\Services' + ServiceName,'Description',Description);
			CloseServiceHandle(hService)
		end;
        CloseServiceHandle(hSCM)
	end
end;

function RemoveService(ServiceName: string) : boolean;
var
	hSCM	: HANDLE;
	hService: HANDLE;
begin
	hSCM := OpenServiceManager();
	Result := false;
	if hSCM <> 0 then begin
		hService := OpenService(hSCM,ServiceName,SERVICE_DELETE);
        if hService <> 0 then begin
            Result := DeleteService(hService);
            CloseServiceHandle(hService)
		end;
        CloseServiceHandle(hSCM)
	end
end;

function StartService(ServiceName: string) : boolean;
var
	hSCM	: HANDLE;
	hService: HANDLE;
begin
	hSCM := OpenServiceManager();
	Result := false;
	if hSCM <> 0 then begin
		hService := OpenService(hSCM,ServiceName,SERVICE_START);
        if hService <> 0 then begin
        	Result := StartNTService(hService,0,0);
            CloseServiceHandle(hService)
		end;
        CloseServiceHandle(hSCM)
	end;
end;

function StopService(ServiceName: string) : boolean;
var
	hSCM	: HANDLE;
	hService: HANDLE;
	Status	: SERVICE_STATUS;
begin
	hSCM := OpenServiceManager();
	Result := false;
	if hSCM <> 0 then begin
		hService := OpenService(hSCM,ServiceName,SERVICE_STOP);
        if hService <> 0 then begin
        	Result := ControlService(hService,SERVICE_CONTROL_STOP,Status);
            CloseServiceHandle(hService)
		end;
        CloseServiceHandle(hSCM)
	end;
end;

function IsServiceRunning(ServiceName: string) : boolean;
var
	hSCM	: HANDLE;
	hService: HANDLE;
	Status	: SERVICE_STATUS;
begin
	hSCM := OpenServiceManager();
	Result := false;
	if hSCM <> 0 then begin
		hService := OpenService(hSCM,ServiceName,SERVICE_QUERY_STATUS);
    	if hService <> 0 then begin
			if QueryServiceStatus(hService,Status) then begin
				Result :=(Status.dwCurrentState = SERVICE_RUNNING)
        	end;
            CloseServiceHandle(hService)
		    end;
        CloseServiceHandle(hSCM)
	end
end;

// #######################################################################################
// create an entry in the services file
// #######################################################################################
function SetupService(service, port, comment: string) : boolean;
var
	filename	: string;
	s			: string;
	lines		: TArrayOfString;
	n			: longint;
	i			: longint;
	errcode		: integer;
	servnamlen	: integer;
	error		: boolean;
begin
	if UsingWinNT() = true then
		filename := ExpandConstant('{sys}\drivers\etc\services')
	else
		filename := ExpandConstant('{win}\services');

	if LoadStringsFromFile(filename,lines) = true then begin
		Result		:= true;
		n			:= GetArrayLength(lines) - 1;
		servnamlen	:= Length(service);
		error		:= false;

		for i:=0 to n do begin
			if Copy(lines[i],1,1) <> '#' then begin
				s := Copy(lines[i],1,servnamlen);
				if CompareText(s,service) = 0 then
					exit; // found service-entry

				if Pos(port,lines[i]) > 0 then begin
					error := true;
					lines[i] := '#' + lines[i] + '   # disabled because collision with  ' + service + ' service';
				end;
			end
			else if CompareText(Copy(lines[i],2,servnamlen),service) = 0 then begin
				// service-entry was disabled
				Delete(lines[i],1,1);
				Result := SaveStringsToFile(filename,lines,false);
				exit;
			end;
		end;

		if error = true then begin
			// save disabled entries
			if SaveStringsToFile(filename,lines,false) = false then begin
				Result := false;
				exit;
			end;
		end;

		// create new service entry
		s := service + '       ' + port + '           # ' + comment + #13#10;
		if SaveStringToFile(filename,s,true) = false then begin
			Result := false;
			exit;
		end;

		if error = true then begin
			MsgBox('the ' + service + ' port was already used. The old service is disabled now. You should check the services file manually now.',mbInformation,MB_OK);
			//InstExec('notepad.exe',filename,GetCurrentDir(),true,false,SW_SHOWNORMAL,errcode);
		end;
	end
	else
		Result := false;
end;

// #######################################################################################
// version functions
// #######################################################################################
function CheckVersion(Filename : string;hh,hl,lh,ll : integer) : boolean;
var
	VersionMS	: cardinal;
	VersionLS	: cardinal;
	CheckMS		: cardinal;
	CheckLS		: cardinal;
begin
	if GetVersionNumbers(Filename,VersionMS,VersionLS) = false then
		Result := false
	else begin
		CheckMS := (hh shl $10) or hl;
		CheckLS := (lh shl $10) or ll;
		Result := (VersionMS > CheckMS) or ((VersionMS = CheckMS) and (VersionLS >= CheckLS));
	end;
end;

// Some examples for version checking
function NeedShellFolderUpdate() : boolean;
begin
	Result := CheckVersion('ShFolder.dll',5,50,4027,300) = false;
end;

function NeedVCRedistUpdate() : boolean;
begin
	Result := (CheckVersion('mfc42.dll',6,0,8665,0) = false)
		or (CheckVersion('msvcrt.dll',6,0,8797,0) = false)
		or (CheckVersion('comctl32.dll',5,80,2614,3600) = false);
end;

function NeedHTMLHelpUpdate() : boolean;
begin
	Result := CheckVersion('hh.exe',4,72,0,0) = false;
end;

function NeedWinsockUpdate() : boolean;
begin
	Result := (UsingWinNT() = false) and (CheckVersion('mswsock.dll',4,10,0,1656) = false);
end;

function NeedDCOMUpdate() : boolean;
begin
	Result := (UsingWinNT() = false) and (CheckVersion('oleaut32.dll',2,30,0,0) = false);
end;

// function IsServiceInstalled(ServiceName: string) : boolean;
// function IsServiceRunning(ServiceName: string) : boolean;
// function InstallService(FileName, ServiceName, DisplayName, Description : string;ServiceType,StartType :cardinal) : boolean;
// function RemoveService(ServiceName: string) : boolean;
// function StartService(ServiceName: string) : boolean;
// function StopService(ServiceName: string) : boolean;

// function SetupService(service, port, comment: string) : boolean;

// function CheckVersion(Filename : string;hh,hl,lh,ll : integer) : boolean;

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
