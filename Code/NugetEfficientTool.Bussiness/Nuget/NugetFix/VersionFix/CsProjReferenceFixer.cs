using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// CsProj文件版本修复器
    /// </summary>
    public class CsProjReferenceFixer : NugetReferenceFixer
    {
        public CsProjReferenceFixer(XDocument xDocument, string csProjPath, IEnumerable<NugetFixStrategy> nugetFixStrategies)
            : base(xDocument, nugetFixStrategies)
        {
            _csProjPath = csProjPath;
        }

        private readonly string _csProjPath;

        /// <summary>
        /// 执行修复
        /// </summary>
        /// <returns>返回修复后的文档内容</returns>
        public override XDocument Fix()
        {
            var fixedDocument = base.Fix();
            if (SucceedStrategies.Any() && IsComponentCsproj(fixedDocument.Root))
            {
                AddComponentVersion(fixedDocument.Root);
            }
            return fixedDocument;
        }

        private bool IsComponentCsproj(XElement rootElement)
        {
            var sdkAttribute = rootElement?.Attribute("Sdk");
            return sdkAttribute != null;
        }

        private void AddComponentVersion(XElement rootElement)
        {
            var propertyGroups = rootElement.Elements("PropertyGroup").ToList();
            var componentVersionElement = propertyGroups.SelectMany(i => i.Elements("Version")).FirstOrDefault();
            if (componentVersionElement != null)
            {
                var componentVersion = componentVersionElement.Value;
                //获取版本中的数字
                var versionNumbers = NumberVersionRegex.Matches(componentVersion);
                //判断版本是否以数字结尾
                var isVersionNumberEnd = versionNumbers.Count > 0 &&
                                         componentVersion.EndsWith(versionNumbers[versionNumbers.Count - 1].Value);
                string newComponentVersion;
                if (isVersionNumberEnd)
                {
                    //数字结尾，版本+1
                    var versionEndNumber = versionNumbers[versionNumbers.Count - 1].Value;
                    var newVersionEnd = Convert.ToInt32(versionEndNumber) + 1;
                    var versionStart = componentVersion.Substring(0, componentVersion.Length - versionEndNumber.Length);
                    newComponentVersion = $"{versionStart}{newVersionEnd}";
                }
                else
                {
                    newComponentVersion = $"{componentVersion}1";
                }
                componentVersionElement.SetValue(newComponentVersion);
                //添加输出日志
                Log = StringSplicer.SpliceWithNewLine(Log, $"    - 升级组件版本至{newComponentVersion}");
            }
        }
        private static readonly Regex NumberVersionRegex = new Regex(@"[0-9]+");
        private static readonly Regex VersionRegex = new Regex(@"(?=.*)(\.[0-9]+){2,3}.[-0-9a-zA-Z]+");
        protected override bool FixDocumentByStrategy(NugetFixStrategy nugetFixStrategy)
        {
            //以PackageReference为主
            var references = CsProj.GetNugetReferences(Document).Where(x =>
            {
                var nugetInfo = CsProj.GetNugetInfo(x);
                return nugetInfo.Name == nugetFixStrategy.NugetName &&
                       nugetInfo.Version != nugetFixStrategy.NugetVersion;
            }).ToList();
            if (!references.Any())
            {
                return false;
            }
            //忽略修复
            if (nugetFixStrategy.NugetVersion == NugetVersion.IgnoreFix)
            {
                return true;
            }

            for (int i = 0; i < references.Count; i++)
            {
                var reference = references[i];
                if (i != 0)
                {
                    reference.Remove();
                    continue;
                }
                if (reference.Name.LocalName == CsProjConst.PackageReferenceName)
                {
                    FixPackageReference(reference, nugetFixStrategy);
                }
                else
                {
                    FixNugetReference(reference, nugetFixStrategy);
                }
            }
            return true;
        }

        /// <summary>
        /// 修复PackageReference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="nugetFixStrategy"></param>
        private void FixPackageReference(XElement reference, NugetFixStrategy nugetFixStrategy)
        {
            var removeAttributeList = new[] { "Private", "HintPath" };
            //删除元素
            if (reference.Elements().Any())
            {
                foreach (var attribute in removeAttributeList)
                {
                    reference.Element(attribute)?.Remove();
                }
            }
            //删除属性
            foreach (var attribute in removeAttributeList)
            {
                // 设置为 null 将删除属性
                reference.SetAttributeValue(attribute, null);
            }

            Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
            reference.SetAttributeValue(CsProjConst.IncludeAttribute, nugetFixStrategy.NugetName);
            var versionElement = reference.Elements().FirstOrDefault(element => element.Name.LocalName == CsProjConst.VersionElementName);
            if (versionElement != null)
            {
                versionElement.SetValue(nugetFixStrategy.NugetVersion);
            }
            else
            {
                reference.SetAttributeValue(CsProjConst.VersionAttribute, nugetFixStrategy.NugetVersion);
            }
        }

        private void FixNugetReference(XElement reference, NugetFixStrategy nugetFixStrategy)
        {
            Log = StringSplicer.SpliceWithNewLine(Log, $"    - 将 {nugetFixStrategy.NugetName} 设定为 {nugetFixStrategy.NugetVersion}");
            ReplaceReferenceToPackageReference(reference, nugetFixStrategy.NugetName, nugetFixStrategy.NugetVersion);
        }

        /// <summary>
        /// 将Reference替换为PackageReference
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="nugetName"></param>
        /// <param name="nugetVersion"></param>
        private void ReplaceReferenceToPackageReference(XElement reference, string nugetName, string nugetVersion)
        {
            //创建元素时，添加xmlns，用于修复xmlns=""空值的问题
            var xmlns = CsProj.GetXmlns(Document);
            var xElement = new XElement(xmlns + CsProjConst.PackageReferenceName);
            xElement.SetAttributeValue(CsProjConst.IncludeAttribute, nugetName);
            xElement.SetAttributeValue(CsProjConst.VersionAttribute, nugetVersion);
            if (reference.NextNode is XElement nextElement)
            {
                reference.Remove();
                nextElement.AddBeforeSelf(xElement);
            }
            else if (reference.PreviousNode is XElement previousElement)
            {
                reference.Remove();
                previousElement.AddAfterSelf(xElement);
            }
            else if (reference.Parent is XElement parentElement)
            {
                reference.Remove();
                parentElement.AddFirst(xElement);
            }
            else
            {
                throw new InvalidOperationException($"{reference}未能完成替换到目标{nugetName}-{nugetVersion}");
            }
            xElement.SetAttributeValue(CsProjConst.XmlnsAttribute, null);
        }

    }
}