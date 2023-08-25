using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Packages文件版本修复器
    /// </summary>
    public class PackagesReferenceFixer : NugetReferenceFixer
    {
        public PackagesReferenceFixer(XDocument xDocument, string packageFile, IEnumerable<NugetFixStrategy> nugetFixStrategies) : base(
            xDocument, nugetFixStrategies)
        {
            _packageFile = packageFile;
        }

        /// <summary>
        /// 使用指定的策略进行修复
        /// </summary>
        /// <param name="nugetFixStrategy">修复策略</param>
        /// <returns>策略是否有效执行</returns>
        protected override bool FixDocumentByStrategy(NugetFixStrategy nugetFixStrategy)
        {
            if (ReferenceEquals(nugetFixStrategy, null)) throw new ArgumentNullException(nameof(nugetFixStrategy));
            var rootElement = Document.Root;
            if (rootElement == null)
            {
                return false;
            }
            var packageElementList = rootElement.Elements()
                .Where(x => x.Attribute(PackagesConfig.IdAttribute)?.Value == nugetFixStrategy.NugetName).ToList();
            if (!packageElementList.Any())
            {
                return false;
            }
            //package中，如果只有一条nuget引用且版本一致，则不用修复
            if (packageElementList.Count == 1 && packageElementList[0].Attribute(PackagesConfig.VersionAttribute)?.Value == nugetFixStrategy.NugetVersion)
            {
                return false;
            }
            if (nugetFixStrategy.NugetVersion == NugetVersions.IgnoreFix)
            {
                return true;
            }

            var targetFramework = packageElementList.First().Attribute(PackagesConfig.TargetFrameworkAttribute)?.Value;
            for (var i = 0; i < packageElementList.Count; i++)
            {
                //没有DLLPath，说明目标引用是PackageReference，则Package中的对应reference可以删除
                var packageElement = packageElementList[i];
                if (nugetFixStrategy.NugetDllInfo == null || string.IsNullOrEmpty(nugetFixStrategy.NugetDllInfo.DllPath))
                {
                    packageElement.Remove();
                    Log = StringSplicer.SpliceWithNewLine(Log, $"    - 因目标为PackageReference，删除{nugetFixStrategy.NugetName}{nugetFixStrategy.NugetVersion}");
                    continue;
                }
                //保留一个Reference即可
                if (i == 0)
                {
                    packageElement.SetAttributeValue(PackagesConfig.IdAttribute, nugetFixStrategy.NugetName);
                    packageElement.SetAttributeValue(PackagesConfig.VersionAttribute,
                        nugetFixStrategy.NugetVersion);
                    packageElement.SetAttributeValue(PackagesConfig.TargetFrameworkAttribute, targetFramework);
                    Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                    continue;
                }
                packageElement.Remove();
            }
            return true;
        }
        private readonly string _packageFile;
    }
}