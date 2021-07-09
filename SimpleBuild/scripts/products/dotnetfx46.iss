// requires Windows 7 Service Pack 1, Windows 8.1, Windows Server 2008 R2 SP1, Windows Server 2012, Windows Server 2012 R2
// express setup (downloads and installs the components depending on your OS) if you want to deploy it locally download the full installer on website below
// https://www.microsoft.com/en-US/download/details.aspx?id=53345

[CustomMessages]
dotnetfx46_title=.NET Framework 4.6.2
dotnetfx46_size=59 MB
//固定英文安装语言
lcid=1033
depdownload_memo_title=Download dependencies
depinstall_memo_title=Install dependencies
depinstall_title=Installing dependencies
depinstall_description=Please wait while Setup installs dependencies on your computer.
depinstall_status=Installing %1...
depinstall_missing=%1 must be installed before setup can continue. Please install %1 and run Setup again.
depinstall_error=An error occured while installing the dependencies. Please restart the computer and run the setup again or install the following dependencies manually:%n
isxdl_langfile=

[Code]
const
	dotnetfx46_url = 'http://download.microsoft.com/download/D/5/C/D5C98AB0-35CC-45D9-9BA5-B18256BA2AE6/NDP462-KB3151802-Web.exe';

procedure dotnetfx46(minVersion: Integer);
begin
	if (dotnetfxspversion(NetFx4x, '') < minVersion) then
		AddProduct('dotnetfx46.exe',
			'/lcid ' + CustomMessage('lcid') + ' /passive /norestart',
			CustomMessage('dotnetfx46_title'),
			CustomMessage('dotnetfx46_size'),
			dotnetfx46_url,
			false, false, false);
end;
