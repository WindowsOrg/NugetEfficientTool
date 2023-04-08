using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget替换配置
    /// </summary>
    public static class NugetFixConfigs
    {
        private const string UserOperationSection = "UserOperation";

        /// <summary>
        /// 当前解决方案
        /// </summary>
        private const string NugetFixKey = "NugetFix";
        public static string GetNugetFixPath()
        {
            var value = IniFileHelper.IniReadValue(UserOperationSection, NugetFixKey);
            return value;
        }
        public static void SaveNugetFixPath(string fixPath)
        {
            IniFileHelper.IniWriteValue(UserOperationSection, NugetFixKey, fixPath);
        }

        /// <summary>
        /// Nuget源路径
        /// </summary>
        private const string NugetSourceUriKey = "NugetSourceUri";
        public static string GetNugetSourceUri()
        {
            var value = IniFileHelper.IniReadValue(UserOperationSection, NugetSourceUriKey);
            return value;
        }
        public static void SaveNugetSourceUri(string nugetSourceUri)
        {
            IniFileHelper.IniWriteValue(UserOperationSection, NugetSourceUriKey, nugetSourceUri);
        }
    }
}
