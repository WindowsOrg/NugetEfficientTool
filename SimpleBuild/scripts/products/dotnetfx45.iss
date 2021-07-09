// requires Windows 7 Service Pack 1, Windows 8, Windows 8.1, Windows Server 2008 R2 SP1, Windows Server 2008 Service Pack 2, Windows Server 2012, Windows Server 2012 R2, Windows Vista Service Pack 2
// express setup (downloads and installs the components depending on your OS) if you want to deploy it locally download the full installer on website below
// https://www.microsoft.com/en-us/download/details.aspx?id=42642

[CustomMessages]
dotnetfx45_title=.NET Framework 4.5.2
dotnetfx45_size=68 MB
//固定英文安装语言
lcid=1033
depdownload_memo_title=Download dependencies
depinstall_memo_title=Install dependencies
depinstall_title=Installing dependencies
depinstall_description=Please wait while Setup installs dependencies on your computer.
depinstall_status=Installing %1...
depinstall_missing=%1 must be installed before setup can continue. Please install %1 and run Setup again.
depinstall_error=An error occured while installing the dependencies. Please restart the computer and run the setup again or install the following dependencies manually:%n
isxdl_langfile=""

[Code]
const
	dotnetfx45_url = 'http://download.microsoft.com/download/B/4/1/B4119C11-0423-477B-80EE-7A474314B347/NDP452-KB2901954-Web.exe';

procedure dotnetfx45(minVersion: Integer);
begin
	if (dotnetfxspversion(NetFx4x, '') < minVersion) then
		AddProduct('dotnetfx45.exe',
			'/lcid ' + CustomMessage('lcid') + ' /passive /norestart',
			CustomMessage('dotnetfx45_title'),
			CustomMessage('dotnetfx45_size'),
			dotnetfx45_url,
			false, false, false);
end;
