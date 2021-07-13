using System;
using System.Collections.Generic;
using System.Linq;

namespace NugetEfficientTool.Business
{
    public static class NugetReplaceCacheManager
    {

        #region 替换记录

        private static List<ReplacedNugetInfo> _replacedNugetInfos;
        /// <summary>
        /// 记录替换记录
        /// </summary>
        /// <param name="solutionFile"></param>
        /// <param name="nugetName"></param>
        /// <returns></returns>
        public static ReplacedNugetInfo GetReplacedNugetInfo(string solutionFile, string nugetName)
        {
            if (_replacedNugetInfos == null)
            {
                _replacedNugetInfos = UserOperationConfigHelper.GetReplaceRecords();
            }
            var replacedNugetInfo = _replacedNugetInfos.FirstOrDefault(i => i.SolutionFile == solutionFile && i.Name == nugetName);
            return replacedNugetInfo;
        }

        public static void ClearReplacedNugetInfo(string solutionFile, string name)
        {
            var nugetInfo = GetReplacedNugetInfo(solutionFile, name);
            if (nugetInfo != null)
            {
                _replacedNugetInfos.Remove(nugetInfo);
                UserOperationConfigHelper.SaveReplaceRecords(_replacedNugetInfos);
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
                _replacedNugetInfos.Remove(nugetInfo);
            }
            _replacedNugetInfos.Add(replacedNugetInfo);
            UserOperationConfigHelper.SaveReplaceRecords(_replacedNugetInfos);
        }

        #endregion
    }
}
