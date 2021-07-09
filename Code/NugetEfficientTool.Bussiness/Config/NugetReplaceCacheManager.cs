using System;
using System.Collections.Generic;
using System.Linq;

namespace NugetEfficientTool.Business
{
    public static class NugetReplaceCacheManager
    {

        #region 替换记录
        private static readonly List<ReplacedNugetInfo> ReplacedNugetInfos = new List<ReplacedNugetInfo>();
        /// <summary>
        /// 记录替换记录
        /// </summary>
        /// <param name="solutionFile"></param>
        /// <param name="nugetName"></param>
        /// <returns></returns>
        public static ReplacedNugetInfo GetReplacedNugetInfo(string solutionFile, string nugetName)
        {
            var replacedNugetInfo =
                ReplacedNugetInfos.FirstOrDefault(i => i.SolutionFile == solutionFile && i.Name == nugetName);
            return replacedNugetInfo;
        }

        public static void ClearReplacedNugetInfo(string solutionFile, string name)
        {
            var nugetInfo = GetReplacedNugetInfo(solutionFile, name);
            if (nugetInfo != null)
            {
                ReplacedNugetInfos.Remove(nugetInfo);
            }
        }
        /// <summary>
        /// 获取替换记录
        /// </summary>
        /// <param name="replacedNugetInfo"></param>
        public static void SaveReplacedNugetInfo(ReplacedNugetInfo replacedNugetInfo)
        {
            var nugetInfo = GetReplacedNugetInfo(replacedNugetInfo.SolutionFile, replacedNugetInfo.Name);
            if (nugetInfo != null)
            {
                ReplacedNugetInfos.Remove(nugetInfo);
            }
            ReplacedNugetInfos.Add(replacedNugetInfo);
        }

        #endregion
    }
}
