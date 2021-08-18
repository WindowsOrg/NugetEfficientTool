using System;
using Microsoft.Win32;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// IE浏览器版本号帮助类
    /// </summary>
    public static class IEVersionHelper
    {
        /// <summary>
        /// 获取IE主版本号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetMajorVersion(string text)
        {
            var majorVersion = string.Empty;

            var detailVersion = GetDetailVersion(text);
            if (!string.IsNullOrWhiteSpace(detailVersion))
            {
                if (detailVersion.IndexOf(".", StringComparison.Ordinal) is int connectedCharFirstIndex && connectedCharFirstIndex > -1)
                {
                    majorVersion = detailVersion.Substring(0, connectedCharFirstIndex);
                }
                else
                {
                    majorVersion = detailVersion;
                }
            }

            return majorVersion;
        }

        /// <summary>
        /// 获取IE详细版本号
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetDetailVersion(string text)
        {
            //通过注册表获取用户IE版本号
            RegistryKey mainKey = Registry.LocalMachine;
            RegistryKey subKey = mainKey.OpenSubKey(text);

            var versionNumber = subKey?.GetValue("svcVersion")?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(versionNumber))
            {
                versionNumber = subKey?.GetValue("svcUpdateVersion")?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(versionNumber))
                {
                    versionNumber = subKey?.GetValue("Version")?.ToString() ?? string.Empty;
                }
            }
            return versionNumber;
        }
    }
}
