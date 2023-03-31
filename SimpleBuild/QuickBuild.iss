#define MyAppName "NugetEfficientTool"
#define MyAppChineseName "Nuget¹¤¾ß"
#define MyAppVersion "1.0.3"
#define MyAppPublisher "Kybs0"
#define MyAppCopyright "Kybs0"
#define MyAppURL "https://www.cnblogs.com/kybs0/"
#define MyAppExeName "NugetEfficientTool.exe"
#define RootPath ".."
#define CurrentDateTimeString GetDateTimeString('mmdd', '', '');

[Setup]
AppId={{60167630-A054-41B7-82BC-945C7584630D}}
AppName={#MyAppChineseName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppChineseName} {#MyAppVersion}
AppCopyright={#MyAppCopyright}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoCopyright={#MyAppCopyright}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf}\{#MyAppName}
;menu displayName
DefaultGroupName={#MyAppChineseName}
OutputDir=..\SimpleBuild\outputFile
OutputBaseFilename={#MyAppChineseName}_{#MyAppVersion}.{#CurrentDateTimeString}
SetupIconFile={#RootPath}\Code\NugetEfficientTool\icon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#RootPath}\Code\bin\Debug\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#RootPath}\Code\bin\Debug\*"; Excludes: "*.bak,*.xml,*.pdb,*.dll.config,*.ax,*\Log\*";DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppChineseName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppChineseName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppChineseName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppChineseName}}";Flags: nowait postinstall skipifsilent   

[CustomMessages]
DependenciesDir=MyProgramDependencies
WindowsServicePack=Windows %1 Service Pack %2