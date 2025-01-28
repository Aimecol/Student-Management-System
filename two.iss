#define myDir "D:\Data Warehouse" ; output folder
#define myDefaultDir "D:\documents\Group5\student\sms\bin\Debug\net9.0-windows" ; App folder
#define myAppVersion "1.0.2" ; Version of the app
#define myAppName "Student management System" ; App name

[Setup]
AppName={#myAppName}
AppVersion={#myAppVersion}
AppVerName={#myAppName} {#myAppVersion}
DefaultDirName={localappdata}\SMS
DefaultGroupName=SMS
OutputDir={#myDir}
OutputBaseFilename={#myAppName}-{#myAppVersion}-setup
SetupIconFile={#myDefaultDir}\group.ico
Compression=lzma
SolidCompression=yes

[Files]
Source: "{#myDefaultDir}\*"; DestDir: "{app}"; Flags: recursesubdirs
Source: "{#myDefaultDir}\students.db"; DestDir: "{app}"; Flags: ignoreversion; Permissions: users-modify

[Tasks]
Name: "startmenu"; Description: "Create a Start Menu Folder"; GroupDescription: "Additional Icons"; Flags: unchecked
Name: "desktopicon"; Description: "Create a Desktop Icon"; GroupDescription: "Additional Icons"; Flags: unchecked

[Icons]
Name: "{group}\{#myAppName}"; Filename: "{app}\sms.exe"; IconFilename: "{app}\group.ico"
Name: "{userdesktop}\{#myAppName}"; Filename: "{app}\sms.exe"; IconFilename: "{app}\group.ico"; Tasks: desktopicon
Name: "{group}\Uninstall {#myAppName}"; Filename: "{uninstallexe}"; Tasks: startmenu
Name: "{autodesktop}\{#myAppName}"; Filename: "{app}\sms.exe"; IconFilename: "{app}\group.ico"

[Run]
Filename: "{app}\sms.exe"; Description: "Run Student Management System"; Flags: nowait postinstall