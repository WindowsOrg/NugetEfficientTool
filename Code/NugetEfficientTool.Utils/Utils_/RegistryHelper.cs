using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 注册表
    /// </summary>
    public static class RegistryHelper
    {
        public static bool ModifyCurrentUserRegistryKey(string registerPath, string exeName, string value)
        {
            RegistryKey currentUserKey = null;
            RegistryKey subKey = null;
            try
            {
                currentUserKey = Registry.CurrentUser;
                subKey = GetSubKey(currentUserKey, registerPath);

                if (subKey != null)
                {
                    subKey.SetValue(exeName, value, RegistryValueKind.DWord);
                    subKey.Close();
                    subKey.Dispose();
                }
            }
            catch (Exception)
            {
                subKey?.Close();
                subKey?.Dispose();
                return false;
            }
            currentUserKey?.Close();
            currentUserKey?.Dispose();
            return true;
        }

        private static RegistryKey GetSubKey(RegistryKey currentUserKey, string registerPath)
        {
            var subKey = currentUserKey.OpenSubKey(registerPath, true);
            if (subKey == null)
            {
                subKey = currentUserKey.CreateSubKey(registerPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }

            return subKey;
        }

        public static bool DeleteCurrentUserRegistryKey(string registerPath, string key)
        {
            RegistryKey currentUserKey = null;
            RegistryKey subKey = null;
            try
            {
                currentUserKey = Registry.CurrentUser;
                subKey = GetSubKey(currentUserKey, registerPath);

                if (subKey != null)
                {
                    subKey.DeleteValue(key, false);
                    subKey.Close();
                    subKey.Dispose();
                }
            }
            catch (Exception)
            {
                subKey?.Close();
                subKey?.Dispose();
                return false;
            }
            currentUserKey?.Close();
            currentUserKey?.Dispose();
            return true;
        }

        public static bool DeleteCurrentUserRegistryPath(string registerPath)
        {
            RegistryKey currentUserKey = null;
            try
            {
                currentUserKey = Registry.CurrentUser;
                currentUserKey.DeleteSubKeyTree(registerPath,true);
            }
            catch (Exception)
            {
                return false;
            }
            currentUserKey?.Close();
            currentUserKey?.Dispose();
            return true;
        }
    }
}
