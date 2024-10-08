﻿using System.Collections.Generic;
using System.Linq;

namespace NugetEfficientTool.Business
{
    public static class NugetReplaceCacheManager
    {

        #region 项目

        /// <summary>
        /// 保存或者更新项目
        /// </summary>
        /// <param name="id"></param>
        /// <param name="solutionFile"></param>
        public static void SaveOrUpdateSolution(string id, string solutionFile)
        {
            var solutions = NugetReplaceConfigs.GetSolutions();
            var projectSolution = solutions.FirstOrDefault(i => i.Id == id);
            if (projectSolution != null)
            {
                projectSolution.SolutionFile = solutionFile;
            }
            else
            {
                solutions.Add(new ProjectSolution(){Id = id,SolutionFile = solutionFile});
            }
            NugetReplaceConfigs.SaveSolutions(solutions);
        }

        #endregion

        #region 替换记录

        public static List<ReplacedNugetInfo> GetReplacedNugetInfos(string projectId)
        {
            return NugetReplaceConfigs.GetReplaceRecords(projectId);
        }

        /// <summary>
        /// 获取指定记录替换记录
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="solutionFile">解决方案</param>
        /// <param name="nugetName"></param>
        /// <returns></returns>
        public static ReplacedNugetInfo GetReplacedNugetInfo(string projectId, string solutionFile, string nugetName)
        {
            var replacedNugetInfos = GetReplacedNugetInfos(projectId);
            var replacedNugetInfo = replacedNugetInfos.FirstOrDefault(i => i.SolutionFile == solutionFile && i.Name == nugetName);
            return replacedNugetInfo;
        }

        /// <summary>
        /// 清除指定Nuget替换记录信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="solutionFile">解决方案</param>
        /// <param name="nugetName"></param>
        public static void ClearReplacedNugetInfo(string projectId, string solutionFile, string nugetName)
        {
            var replacedNugetInfos = GetReplacedNugetInfos(projectId);
            var replacedNugetInfo = replacedNugetInfos.FirstOrDefault(i => i.SolutionFile == solutionFile && i.Name == nugetName);
            if (replacedNugetInfo != null)
            {
                replacedNugetInfos.Remove(replacedNugetInfo);
                NugetReplaceConfigs.SaveReplaceRecords(projectId, replacedNugetInfos);
            }
        }
        public static void ClearReplacedNugetInfo(string projectId)
        {
            NugetReplaceConfigs.SaveReplaceRecords(projectId, new List<ReplacedNugetInfo>());
        }
        /// <summary>
        /// 获取替换记录
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="replacedNugetInfo"></param>
        public static void SaveReplacedNugetInfo(string projectId, ReplacedNugetInfo replacedNugetInfo)
        {
            var replacedNugetInfos = GetReplacedNugetInfos(projectId);
            var nugetInfo = replacedNugetInfos.FirstOrDefault(i => i.SolutionFile == replacedNugetInfo.SolutionFile && i.Name == replacedNugetInfo.Name);
            if (nugetInfo != null)
            {
                replacedNugetInfos.Remove(nugetInfo);
            }
            //nuget替换后，要按后替换先恢复的原则，进行保存
            replacedNugetInfos.Insert(0, replacedNugetInfo);
            NugetReplaceConfigs.SaveReplaceRecords(projectId, replacedNugetInfos);
        }

        #endregion

    }
}
