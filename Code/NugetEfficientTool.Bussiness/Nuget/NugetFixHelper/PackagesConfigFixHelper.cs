using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NugetEfficientTool.Business;

namespace NugetEfficientTool.Business
{
    public class PackagesConfigFixHelper : NugetConfigFixHelperBase
    {
        public PackagesConfigFixHelper(XDocument xDocument, IEnumerable<NugetFixStrategy> nugetFixStrategies) : base(
            xDocument, nugetFixStrategies)
        {
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
            var packageElementList = rootElement.Elements()
                .Where(x => x.Attribute(PackagesConfig.IdAttribute).Value == nugetFixStrategy.NugetName).ToList();
            if (!packageElementList.Any())
            {
                return false;
            }

            if (nugetFixStrategy.NugetVersion == NugetVersion.IgnoreFix)
            {
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 根据策略，忽略 {nugetFixStrategy.NugetName} 存在的问题");
                return true;
            }

            var targetFramework = packageElementList.First().Attribute(PackagesConfig.TargetFrameworkAttribute).Value;
            for (var i = 0; i < packageElementList.Count; i++)
            {
                if (i == 0)
                {
                    var firstPackageElement = packageElementList[i];
                    firstPackageElement.SetAttributeValue(PackagesConfig.IdAttribute, nugetFixStrategy.NugetName);
                    firstPackageElement.SetAttributeValue(PackagesConfig.VersionAttribute,
                        nugetFixStrategy.NugetVersion);
                    firstPackageElement.SetAttributeValue(PackagesConfig.TargetFrameworkAttribute, targetFramework);
                    Log = StringSplicer.SpliceWithNewLine(Log,
                        $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                    continue;
                }

                packageElementList[i].Remove();
            }

            if (packageElementList.Count > 1)
            {
                Log = StringSplicer.SpliceWithNewLine(Log,
                    $"    - 删除了 {nugetFixStrategy.NugetName} 的 {packageElementList.Count - 1} 个冲突引用");
            }

            return true;
        }
    }
}