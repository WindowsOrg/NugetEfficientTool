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
    public static class NugetReplaceConfigs
    {
        private const string UserOperationSection = "UserOperation";

        #region 项目

        /// <summary>
        /// 当前解决方案
        /// </summary>
        private const string SolutionsKey = "Solutions";
        public static List<ProjectSolution> GetSolutions()
        {
            var value = IniFileHelper.IniReadValue(UserOperationSection, SolutionsKey);
            if (string.IsNullOrEmpty(value))
            {
                return new List<ProjectSolution>();
            }
            var solutions = JsonConvert.DeserializeObject<List<ProjectSolution>>(value);
            return solutions;
        }

        public static event EventHandler<List<ProjectSolution>> SolutionFileUpdated;
        public static void SaveSolutions(List<ProjectSolution> solutionFiles)
        {
            var jsonData = JsonConvert.SerializeObject(solutionFiles);
            IniFileHelper.IniWriteValue(UserOperationSection, SolutionsKey, jsonData);
            SolutionFileUpdated?.Invoke(null, solutionFiles);
        }

        #endregion

        #region 替换配置

        /// <summary>
        /// Nuget替换配置
        /// </summary>
        private const string NugetReplaceConfigKey = "NugetReplaceConfig";
        public static List<ReplaceNugetConfig> GetNugetReplaceConfig(string projectId)
        {
            var valueJson = IniFileHelper.IniReadValue(UserOperationSection, $"{NugetReplaceConfigKey}_{projectId}");
            if (string.IsNullOrEmpty(valueJson))
            {
                return new List<ReplaceNugetConfig>();
            }
            var replaceNugetConfigs = JsonConvert.DeserializeObject<List<ReplaceNugetConfig>>(valueJson);
            return replaceNugetConfigs;
        }
        public static void SaveNugetReplaceConfig(string projectId, List<ReplaceNugetConfig> replaceNugetConfigs)
        {
            var jsonData = JsonConvert.SerializeObject(replaceNugetConfigs);
            IniFileHelper.IniWriteValue(UserOperationSection, $"{NugetReplaceConfigKey}_{projectId}", jsonData);
        }

        #endregion

        #region 替换记录

        /// <summary>
        /// Nuget替换信息
        /// </summary>
        private const string ReplaceRecordsKey = "ReplaceRecords";
        public static List<ReplacedNugetInfo> GetReplaceRecords(string projectId)
        {
            var valueJson = IniFileHelper.IniReadValue(UserOperationSection, $"{ReplaceRecordsKey}_{projectId}");
            if (string.IsNullOrEmpty(valueJson))
            {
                return new List<ReplacedNugetInfo>();
            }
            var replaceRecords = JsonConvert.DeserializeObject<List<ReplacedNugetInfo>>(valueJson);
            return replaceRecords;
        }
        public static void SaveReplaceRecords(string projectId, List<ReplacedNugetInfo> replaceRecords)
        {
            var jsonData = JsonConvert.SerializeObject(replaceRecords);
            IniFileHelper.IniWriteValue(UserOperationSection, $"{ReplaceRecordsKey}_{projectId}", jsonData);
        }

        #endregion

    }
}
