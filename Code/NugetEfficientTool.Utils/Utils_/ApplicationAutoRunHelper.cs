using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 开机自启动
    /// </summary>
    public class ApplicationAutoRunHelper
    {
        private void SetAppAutoRun(bool autoRun)
        {
            try
            {
                string executablePath = Application.ExecutablePath;
                string exeName = Path.GetFileNameWithoutExtension(executablePath);
                SetAutoRun(autoRun, exeName, executablePath);
            }
            catch (Exception)
            {
            }
        }
        private bool SetAutoRun(bool autoRun, string exeName, string executablePath)
        {
            bool success = SetAutoRun(Registry.CurrentUser, autoRun, exeName, executablePath);
            if (!success)
            {
                success = SetAutoRun(Registry.LocalMachine, autoRun, exeName, executablePath);
            }
            return success;
        }
        private bool SetAutoRun(RegistryKey rootKey, bool autoRun, string exeName, string executablePath)
        {
            try
            {
                RegistryKey autoRunKey = rootKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (autoRunKey == null)
                {
                    autoRunKey = rootKey.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", RegistryKeyPermissionCheck.ReadWriteSubTree);
                }
                if (autoRunKey != null)
                {
                    if (autoRun) //设置开机自启动  
                    {
                        autoRunKey.SetValue(exeName, $"\"{executablePath}\" /background");
                    }
                    else //取消开机自启动  
                    {
                        autoRunKey.DeleteValue(exeName, false);
                    }
                    autoRunKey.Close();
                    autoRunKey.Dispose();
                }
            }
            catch (Exception)
            {
                rootKey.Close();
                rootKey.Dispose();
                return false;
            }
            rootKey.Close();
            rootKey.Dispose();
            return true;
        }
    }
}
