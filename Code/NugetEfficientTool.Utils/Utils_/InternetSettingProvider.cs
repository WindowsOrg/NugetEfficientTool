using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 注册表功能修改项
    /// </summary>
    public class InternetSettingProvider
    {
        /// <summary>
        /// 证书吊销是否检查
        /// </summary>
        /// <param name="isChecked"></param>
        public void SetCertificateVerificationChecked(bool isChecked)
        {
            //检查发行商的证书是否吊销
            //0 开启，512 取消
            var softwarePublishing = @"Software\Microsoft\Windows\CurrentVersion\WinTrust\Trust Providers\Software Publishing";
            RegistryHelper.ModifyCurrentUserRegistryKey(softwarePublishing, "State", isChecked ? "0" : "512 ");

            //检查服务器证书吊销
            //1开启，0关闭
            var internetSettings = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            RegistryHelper.ModifyCurrentUserRegistryKey(internetSettings, "CertificateRevocation", isChecked ? "1" : "0");
        }

        /// <summary>
        /// 设置SSLAndTSL
        /// </summary>
        /// <param name="isOpen"></param>
        public void SetSSLAndTSLState(bool isOpen)
        {
            //使用SSL3.0和TLS1.0
            //var internetSettings = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            //RegistryHelper.ModifyCurrentUserRegistryKey(internetSettings, "SecureProtocols", isChecked ? "2688" : "0");
        }

        /// <summary>
        /// 修改IE安全等级为中
        /// </summary>
        public async void SetInternetProtectLevelNormal()
        {
            //"CurrentLevel"=dword:00011000
            //"MinLevel" = dword:00011000
            //"RecommendedLevel" = dword:00011000
            //var internetSettings = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\3";
            //RegistryHelper.ModifyCurrentUserRegistryKey(internetSettings, "CurrentLevel", "00011000");
            //RegistryHelper.ModifyCurrentUserRegistryKey(internetSettings, "MinLevel", "00011000");
            //RegistryHelper.ModifyCurrentUserRegistryKey(internetSettings, "RecommendedLevel", "00011000");

            dynamic shellObject = Interaction.CreateObject("WScript.Shell", "");
            //打开IE设置-安全
            shellObject.Run("Rundll32.exe shell32.dll,Control_RunDLL inetcpl.cpl,,1");
            await Task.Delay(500);
            //默认级别
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{R}");

            await Task.Delay(100);
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{TAB}");
            shellObject.SendKeys("{A}");

            await Task.Delay(100);
            shellObject.SendKeys("{ENTER}");
        }

        /// <summary>
        /// 重置IE浏览器-删除IE注册表
        /// </summary>
        public void ResetIeByRegistry()
        {
            var internetSettings = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
            RegistryHelper.DeleteCurrentUserRegistryPath(internetSettings);
        }

        /// <summary>
        /// 重置IE浏览器-BAT脚本
        /// </summary>
        public void ResetIeByBat()
        {
            //ResetIeSetting.bat
        }

        /// <summary>
        /// 重置IE浏览器
        /// </summary>
        public async Task ResetInternetSettingAsync()
        {
            dynamic shellObject = Interaction.CreateObject("WScript.Shell", "");
            try
            {
                //打开IE设置-重置浏览器
                shellObject.Run("rundll32.exe inetcpl.cpl ResetIEtoDefaults");
                await Task.Delay(800);
                //选中删除个人设置
                shellObject.SendKeys("{P}");
                await Task.Delay(100);
                //选中确定
                shellObject.SendKeys("{TAB}");
                shellObject.SendKeys("{TAB}");
                shellObject.SendKeys("{R}");
                await Task.Delay(1000);
                //关闭设置
                shellObject.SendKeys("{ENTER}");
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// IE-还原高级设置
        /// </summary>
        public async Task RevertHighSettingAsync()
        {
            dynamic shellObject = Interaction.CreateObject("WScript.Shell", "");
            try
            {
                //打开IE设置-重置浏览器
                shellObject.Run("Rundll32.exe shell32.dll,Control_RunDLL inetcpl.cpl,,6");
                await Task.Delay(800);
                //选中删除个人设置
                shellObject.SendKeys("{TAB}");
                shellObject.SendKeys("{R}");
                await Task.Delay(100);
                //选中确定
                shellObject.SendKeys("{TAB}");
                shellObject.SendKeys("{TAB}");
                shellObject.SendKeys("{A}");
                await Task.Delay(100);
                //关闭设置

                shellObject.SendKeys("{ENTER}");
            }
            catch (Exception)
            {
            }
        }
    }
}
