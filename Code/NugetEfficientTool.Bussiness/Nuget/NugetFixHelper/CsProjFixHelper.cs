using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    public class CsProjFixHelper : NugetConfigFixHelperBase
    {
        public CsProjFixHelper(XDocument xDocument, string csProjPath, IEnumerable<NugetFixStrategy> nugetFixStrategies)
            : base(xDocument, nugetFixStrategies)
        {
            _csProjPath = csProjPath;
        }

        private readonly string _csProjPath;

        private void FixPackageReferences(IEnumerable<XElement> packageReferences, NugetFixStrategy nugetFixStrategy)
        {
            var packageReferenceList = packageReferences.ToList();
            var parentItemGroup = packageReferenceList.First().Parent;
            for (var i = 0; i < packageReferenceList.Count; i++)
            {
                if (i != 0)
                {
                    packageReferenceList[i].Remove();
                    continue;
                }

                var removeAttributeList = new[] {"Private", "HintPath"};

                var firstPackageReference = packageReferenceList[i];
                if (firstPackageReference.Elements().Any())
                {
                    foreach (var attribute in removeAttributeList)
                    {
                        firstPackageReference.Element(attribute)?.Remove();
                    }
                   
                    Log = StringSplicer.SpliceWithNewLine(Log, $"    - 更新了 {nugetFixStrategy.NugetName} 的版本声明格式");
                }

                Log = StringSplicer.SpliceWithNewLine(Log,
                    $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");

                foreach (var attribute in removeAttributeList)
                {
                    // 设置为 null 将删除属性
                    firstPackageReference.SetAttributeValue(attribute, null);
                }

                firstPackageReference.SetAttributeValue(CsProj.IncludeAttribute, nugetFixStrategy.NugetName);
                firstPackageReference.SetAttributeValue(CsProj.VersionAttribute, nugetFixStrategy.NugetVersion);
            }

            if (packageReferenceList.Count > 1)
            {
                Log = StringSplicer.SpliceWithNewLine(Log,
                    $"    - 删除了 {nugetFixStrategy.NugetName} 的 {packageReferenceList.Count - 1} 个冲突引用");
            }
        }

        private void DeleteNugetInfoReferences(IEnumerable<XElement> nugetInfoReferences)
        {
            var nugetInfoReferenceList = nugetInfoReferences.ToList();
            nugetInfoReferenceList.ForEach(x => x.Remove());
        }

        private void FixNugetInfoReferences(IEnumerable<XElement> nugetInfoReferences,
            NugetFixStrategy nugetFixStrategy)
        {
            var nugetInfoReferenceList = nugetInfoReferences.ToList();
            for (var i = 0; i < nugetInfoReferenceList.Count; i++)
            {
                if (i != 0)
                {
                    nugetInfoReferenceList[i].Remove();
                    continue;
                }

                var firstNugetInfoReference = nugetInfoReferenceList[i];
                if (nugetFixStrategy.NugetDllInfo == null)
                {
                    ;
                }

                firstNugetInfoReference.SetAttributeValue(CsProj.IncludeAttribute,
                    nugetFixStrategy.NugetDllInfo.DllFullName);
                Log = StringSplicer.SpliceWithNewLine(Log,
                    $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                var hintPathElementList = firstNugetInfoReference.Elements()
                    .Where(x => x.Name.LocalName == CsProj.HintPathElementName).ToList();
                for (var j = 0; j < hintPathElementList.Count; j++)
                {
                    if (j != 0)
                    {
                        hintPathElementList[i].Remove();
                        continue;
                    }

                    hintPathElementList[i].Value = MakeRelativePath(Path.GetDirectoryName(_csProjPath),
                        nugetFixStrategy.NugetDllInfo.DllPath);
                }

                if (hintPathElementList.Count > 1)
                {
                    Log = StringSplicer.SpliceWithNewLine(Log,
                        $"    - 删除了 {nugetFixStrategy.NugetName} 的 {hintPathElementList.Count - 1} 个子健冲突");
                }
            }

            if (nugetInfoReferenceList.Count > 1)
            {
                Log = StringSplicer.SpliceWithNewLine(Log,
                    $"    - 删除了 {nugetFixStrategy.NugetName} 的 {nugetInfoReferenceList.Count - 1} 个冲突引用");
            }
        }

        /// <summary>
        /// 根据绝对路径生成相对路径
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        /// <returns></returns>
        private static string MakeRelativePath( string fromPath,  string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);
            if (fromUri.Scheme != toUri.Scheme)
            {
                // 不是同一种路径，无法转换成相对路径。
                return toPath;
            }

            if (fromUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase) && !fromPath.EndsWith("/") &&
                !fromPath.EndsWith("\\"))
            {
                // 如果是文件系统，则视来源路径为文件夹。
                fromUri = new Uri(fromPath + Path.DirectorySeparatorChar);
            }

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        protected override bool FixDocumentByStrategy(NugetFixStrategy nugetFixStrategy)
        {
            var packageReferences = CsProj.GetPackageReferences(Document).Where(x =>
                x.Attribute(CsProj.IncludeAttribute).Value == nugetFixStrategy.NugetName);
            var nugetInfoReferences = CsProj.GetNugetInfoReferences(Document).Where(x =>
                CsProj.GetNugetInfoFromNugetInfoReference(x).Name == nugetFixStrategy.NugetName);
            if (!packageReferences.Any() && !nugetInfoReferences.Any())
            {
                return false;
            }

            if (nugetFixStrategy.NugetVersion == NugetVersion.IgnoreFix)
            {
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 根据策略，忽略 {nugetFixStrategy.NugetName} 存在的问题");
                return true;
            }

            if (packageReferences.Any())
            {
                FixPackageReferences(packageReferences, nugetFixStrategy);
                DeleteNugetInfoReferences(nugetInfoReferences);
            }
            else
            {
                FixNugetInfoReferences(nugetInfoReferences, nugetFixStrategy);
            }

            return true;
        }
    }
}