using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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

        /// <summary>
        /// 修复PackageReference
        /// </summary>
        /// <param name="packageReferences"></param>
        /// <param name="nugetFixStrategy"></param>
        private void FixPackageReferences(IEnumerable<XElement> packageReferences, NugetFixStrategy nugetFixStrategy)
        {
            var packageReferenceList = packageReferences.ToList();
            for (var i = 0; i < packageReferenceList.Count; i++)
            {
                if (i != 0)
                {
                    packageReferenceList[i].Remove();
                    continue;
                }

                var removeAttributeList = new[] { "Private", "HintPath" };

                var firstPackageReference = packageReferenceList[i];
                if (firstPackageReference.Elements().Any())
                {
                    foreach (var attribute in removeAttributeList)
                    {
                        firstPackageReference.Element(attribute)?.Remove();
                    }

                    Log = StringSplicer.SpliceWithNewLine(Log, $"    - 更新了 {nugetFixStrategy.NugetName} 的版本声明格式");
                }
                foreach (var attribute in removeAttributeList)
                {
                    // 设置为 null 将删除属性
                    firstPackageReference.SetAttributeValue(attribute, null);
                }

                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                firstPackageReference.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetName);
                var versionElement = firstPackageReference.Elements().FirstOrDefault(element => element.Name.LocalName == CsProjConst.VersionElementName);
                if (versionElement != null)
                {
                    versionElement.SetValue(nugetFixStrategy.NugetVersion);
                }
                else
                {
                    firstPackageReference.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
                }
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

                var currentNugetReference = nugetInfoReferenceList[i];
                if (nugetFixStrategy.NugetDllInfo == null)
                {
                    continue;
                }
                //system引用，会被替换并修改
                //todo 2.System.Runtime.Cache会被删除
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
                if (string.IsNullOrEmpty(nugetFixStrategy.NugetDllInfo.DllPath))
                {
                    var xElement = new XElement(CsProjConst.PackageReferenceName);
                    xElement.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetDllInfo.DllFullName);
                    xElement.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
                    if (currentNugetReference.NextNode is XElement nextElement)
                    {
                        currentNugetReference.Remove();
                        nextElement.AddBeforeSelf(xElement);
                    }
                    else if (currentNugetReference.PreviousNode is XElement previousElement)
                    {
                        currentNugetReference.Remove();
                        previousElement.AddAfterSelf(xElement);
                    }
                    else if (currentNugetReference.Parent is XElement parentElement)
                    {
                        currentNugetReference.Remove();
                        parentElement.AddFirst(xElement);
                    }
                    else
                    {
                        throw new InvalidOperationException($"{currentNugetReference}未能完成替换到目标{nugetFixStrategy.NugetName}-{nugetFixStrategy.NugetVersion}");
                    }
                }
                else
                {
                    currentNugetReference.SetAttributeValue(CsProjConst.IncludeAttribute,
                        nugetFixStrategy.NugetDllInfo.DllFullName);
                    var hintPathElementList = currentNugetReference.Elements()
                        .Where(x => x.Name.LocalName == CsProjConst.HintPathElementName).ToList();
                    for (var j = 0; j < hintPathElementList.Count; j++)
                    {
                        if (j != 0)
                        {
                            hintPathElementList[j].Remove();
                            continue;
                        }
                        hintPathElementList[j].Value = MakeRelativePath(Path.GetDirectoryName(_csProjPath),
                            nugetFixStrategy.NugetDllInfo.DllPath);
                    }
                    if (hintPathElementList.Count > 1)
                    {
                        Log = StringSplicer.SpliceWithNewLine(Log, $"    - 删除了 {nugetFixStrategy.NugetName} 的 {hintPathElementList.Count - 1} 个子健冲突");
                    }
                }
            }

            if (nugetInfoReferenceList.Count > 1)
            {
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 删除了 {nugetFixStrategy.NugetName} 的 {nugetInfoReferenceList.Count - 1} 个冲突引用");
            }
        }

        private static void AddPackageReference(NugetFixStrategy nugetFixStrategy, XElement firstNugetInfoReference)
        {
            var xElement = new XElement(CsProjConst.PackageReferenceName);
            xElement.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetDllInfo.DllFullName);
            xElement.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
            if (firstNugetInfoReference.NextNode is XElement nextElement)
            {
                firstNugetInfoReference.Remove();
                nextElement.AddBeforeSelf(xElement);
            }
            else if (firstNugetInfoReference.PreviousNode is XElement previousElement)
            {
                firstNugetInfoReference.Remove();
                previousElement.AddAfterSelf(xElement);
            }
            else if (firstNugetInfoReference.Parent is XElement parentElement)
            {
                firstNugetInfoReference.Remove();
                parentElement.AddFirst(xElement);
            }
            else
            {
                throw new InvalidOperationException(
                    $"{firstNugetInfoReference}未能完成替换到目标{nugetFixStrategy.NugetName}-{nugetFixStrategy.NugetVersion}");
            }
        }

        /// <summary>
        /// 根据绝对路径生成相对路径
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toPath"></param>
        /// <returns></returns>
        private static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
            {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (string.IsNullOrEmpty(toPath))
            {
                throw new ArgumentNullException(nameof(toPath));
            }
            ////不同解决方案间的引用，不能跨解决方案，还是要在本解决方案内解决。
            ////所以除了改为相对路径，还需要复制nuget文件到当前解决方案路径下 todo
            //var dllPath = toPath;
            //if (dllPath.Contains("packages"))
            //{
            //    var list = dllPath.Split(new[] { "\\packages\\" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    if (list.Count > 1)
            //    {
            //        dllPath = $"..\\packages\\{list.Last()}";
            //    }
            //}
            //return dllPath;

            //获取相对路径
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
            var packageReferences = CsProj.GetReferences(Document).Where(x => CheckIncludeAttributeContainsName(x, nugetFixStrategy.NugetName)).ToList();
            var nugetInfoReferences = CsProj.GetNugetInfoReferences(Document).Where(x =>
                CsProj.GetNugetInfo(x).Name == nugetFixStrategy.NugetName).ToList();
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
                nugetInfoReferences.RemoveAll(i => packageReferences.Any(package => package != i));
            }
            else
            {
                FixNugetInfoReferences(nugetInfoReferences, nugetFixStrategy);
            }

            return true;
        }

        private bool CheckIncludeAttributeContainsName(XElement x, string nugetName)
        {
            var xAttribute = x.Attribute(CsProjConst.IncludeAttribute);
            if (!(xAttribute?.Value is string includeAttributeValue && !string.IsNullOrEmpty(includeAttributeValue)))
            {
                return false;
            }

            if (includeAttributeValue.Contains(","))
            {
                var includeValues = includeAttributeValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                includeAttributeValue = includeAttributeValue[0].ToString();
            }
            return includeAttributeValue.StartsWith(nugetName);
        }
    }
}