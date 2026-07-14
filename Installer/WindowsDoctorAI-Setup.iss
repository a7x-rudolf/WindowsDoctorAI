; ═══════════════════════════════════════════════════════════════════════
; Windows Doctor AI - Inno Setup Installer Script
; Author: RIDOLF WIDI ALFISA LUMBA
; Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved.
; License: MIT License
; ═══════════════════════════════════════════════════════════════════════

#define AppName "Windows Doctor AI"
#define AppNameShort "WindowsDoctorAI"
#define AppVersion "2.0.0"
#define AppPublisher "RIDOLF WIDI ALFISA LUMBA"
#define AppURL "https://github.com/RidolfAlfisa/WindowsDoctorAI"
#define AppExeName "WindowsDoctorAI.exe"
#define AppCopyright "Copyright © 2025 RIDOLF WIDI ALFISA LUMBA. All Rights Reserved."
#define AppDescription "Premium Windows System Diagnostic and Repair Tool"

[Setup]
; APP IDENTIFICATION
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} v{#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
AppCopyright={#AppCopyright}
AppComments={#AppDescription}
AppContact={#AppPublisher}

; INSTALLATION PATHS
DefaultDirName={autopf}\{#AppNameShort}
DefaultGroupName={#AppName}
DisableProgramGroupPage=no
DisableDirPage=no
AllowNoIcons=yes

; PRIVILEGES & COMPATIBILITY
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=commandline
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
MinVersion=10.0.17763

; INSTALLER APPEARANCE
WizardStyle=modern
WizardResizable=no
SetupIconFile=D:\Project\WindowsDoctorAI\WindowsDoctorAI\Assets\AppIcon.ico
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}

; OUTPUT SETTINGS
OutputDir=D:\Project\WindowsDoctorAI\Installer\Output
OutputBaseFilename=WindowsDoctorAI-v{#AppVersion}-Setup-x64
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes

; VERSION INFO
VersionInfoVersion={#AppVersion}.0
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppDescription}
VersionInfoTextVersion={#AppVersion}
VersionInfoCopyright={#AppCopyright}
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}.0

; UNINSTALL SETTINGS
UninstallFilesDir={app}\uninstall

; DISK SPACE
DiskSpanning=no
UsePreviousAppDir=yes
UsePreviousGroup=yes
UsePreviousTasks=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1
Name: "startupicon"; Description: "Launch {#AppName} on Windows startup"; GroupDescription: "Additional options:"; Flags: unchecked

[Files]
; Main executable
Source: "D:\Project\WindowsDoctorAI\Publish\{#AppExeName}"; DestDir: "{app}"; Flags: ignoreversion

; All other files from Publish folder
Source: "D:\Project\WindowsDoctorAI\Publish\*"; DestDir: "{app}"; Excludes: "*.pdb,{#AppExeName}"; Flags: ignoreversion recursesubdirs createallsubdirs

; DOCUMENTATION FILES
Source: "D:\Project\WindowsDoctorAI\README.md"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "D:\Project\WindowsDoctorAI\LICENSE"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist
Source: "D:\Project\WindowsDoctorAI\CHANGELOG.md"; DestDir: "{app}"; Flags: ignoreversion skipifsourcedoesntexist

[Icons]
; START MENU SHORTCUTS
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\Assets\AppIcon.ico"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"

; DESKTOP SHORTCUT (Optional)
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"; IconFilename: "{app}\Assets\AppIcon.ico"; Tasks: desktopicon

; QUICK LAUNCH (Older Windows only)
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: quicklaunchicon

[Registry]
; STARTUP REGISTRATION (Optional)
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#AppNameShort}"; ValueData: """{app}\{#AppExeName}"""; Flags: uninsdeletevalue; Tasks: startupicon

[Run]
; POST-INSTALLATION ACTIONS
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent shellexec

[UninstallDelete]
; CLEANUP ON UNINSTALL
Type: filesandordirs; Name: "{app}\logs"
Type: filesandordirs; Name: "{app}\temp"

[Code]
function InitializeSetup(): Boolean;
begin
  Result := True;
  
  // Check Windows version
  if not IsWin64 then
  begin
    MsgBox('This application requires 64-bit Windows.' + #13#10 + 
           'Setup will now exit.', mbCriticalError, MB_OK);
    Result := False;
    Exit;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Log installation success
    Log('Windows Doctor AI v2.0.0 installed successfully. Developed by RIDOLF WIDI ALFISA LUMBA.');
  end;
end;

