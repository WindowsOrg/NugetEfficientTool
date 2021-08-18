using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    public static class UserOperationConfigHelper
    {
        private const string UserOperationSection = "UserOperation";
        /// <summary>
        /// 当前解决方案
        /// </summary>
        private const string SolutionFileKey = "SolutionFile";
        public static string GetSolutionFile()
        {
            var value = IniFileHelper.IniReadValue(UserOperationSection, SolutionFileKey);
            return value ?? string.Empty;
        }

        public static event EventHandler<string> SolutionFileUpdated;
        public static void SaveSolutionFile(string solutionFile)
        {
            IniFileHelper.IniWriteValue(UserOperationSection, SolutionFileKey, solutionFile);
            SolutionFileUpdated?.Invoke(null, solutionFile);
        }

        /// <summary>
        /// 当前Nuget替换信息
        /// </summary>
        private const string NugetReplaceConfigKey = "NugetReplaceConfig";
        public static List<ReplaceNugetConfig> GetNugetReplaceConfig()
        {
            var valueJson = IniFileHelper.IniReadValue(UserOperationSection, NugetReplaceConfigKey);
            if (string.IsNullOrEmpty(valueJson))
            {
                return new List<ReplaceNugetConfig>();
            }
            var replaceNugetConfigs = JsonConvert.DeserializeObject<List<ReplaceNugetConfig>>(valueJson);
            return replaceNugetConfigs;
        }
        public static void SaveNugetReplaceConfig(List<ReplaceNugetConfig> replaceNugetConfigs)
        {
            var jsonData = JsonConvert.SerializeObject(replaceNugetConfigs);
            IniFileHelper.IniWriteValue(UserOperationSection, NugetReplaceConfigKey, jsonData);
        }

        /// <summary>
        /// 当前Nuget替换信息
        /// </summary>
        private const string ReplaceRecordsKey = "ReplaceRecords";
        public static List<ReplacedNugetInfo> GetReplaceRecords()
        {
            var valueJson = IniFileHelper.IniReadValue(UserOperationSection, ReplaceRecordsKey);
            if (string.IsNullOrEmpty(valueJson))
            {
                return new List<ReplacedNugetInfo>();
            }
            var replaceRecords = JsonConvert.DeserializeObject<List<ReplacedNugetInfo>>(valueJson);
            return replaceRecords;
        }
        public static void SaveReplaceRecords(List<ReplacedNugetInfo> replaceRecords)
        {
            var jsonData = JsonConvert.SerializeObject(replaceRecords);
            IniFileHelper.IniWriteValue(UserOperationSection, ReplaceRecordsKey, jsonData);
        }
    }
    [DataContract]
    public class ReplaceNugetConfig
    {
        public ReplaceNugetConfig(string nugetName, string sourceCroProjFile)
        {
            Name = nugetName;
            SourceCsprojPath = sourceCroProjFile;
        }

        /// <summary>
        /// Nuget名称
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        /// <summary>
        /// 源代码csproj文件路径(可能是相对路径，也可能是绝对路径)
        /// </summary>
        [DataMember(Name = "SourceCsprojPath")]
        public string SourceCsprojPath { get; set; }
    }
}
