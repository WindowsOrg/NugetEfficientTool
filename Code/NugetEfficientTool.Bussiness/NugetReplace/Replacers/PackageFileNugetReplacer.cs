using System;
using System.Linq;
using System.Xml.Linq;
using Kybs0.Project;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Package文件Nuget替换类
    /// </summary>
    public class PackageFileNugetReplacer : XmlFileNugetReplacer
    {
        private readonly ReplacedFileRecord _replacedFileRecord;
        private readonly string _nugetName;

        public PackageFileNugetReplacer(string packageFile, string nugetName) : base(packageFile)
        {
            _nugetName = nugetName;
        }

        public PackageFileNugetReplacer(string packageFile, ReplacedFileRecord replacedFileRecord) : base(packageFile)
        {
            _replacedFileRecord = replacedFileRecord;
        }
        /// <summary>
        /// 替换Nuget信息
        /// </summary>
        /// <returns></returns>
        public ReplacedFileRecord ReplaceNuget()
        {
            var document = Document;
            var documentRoot = document.Root;
            if (documentRoot == null)
            {
                throw new InvalidOperationException($"document.Root是空的,{XmlFile}");
            }
            var packageElements = documentRoot.Elements().ToList();
            var packageElement = packageElements.FirstOrDefault(x => x.Attribute(PackagesConfig.IdAttribute)?.Value == _nugetName);
            if (packageElement == null)
            {
                return null;
            }
            //获取package引用信息
            var modifiedRecord = new ReplacedFileRecord() { FileName = File, NugetName = _nugetName, };
            modifiedRecord.ModifiedLineIndex = packageElements.IndexOf(packageElement);
            var versionAttribute = packageElement.Attribute(PackagesConfig.VersionAttribute);
            if (versionAttribute == null)
            {
                throw new InvalidOperationException($"package的版本信息是空的，{XmlFile}，{packageElement.Name}");
            }
            modifiedRecord.Version = versionAttribute?.Value;
            modifiedRecord.TargetFramework = packageElement.Attribute(PackagesConfig.TargetFrameworkAttribute)?.Value;
            //删除package引用
            packageElement.Remove();
            SaveFile();
            return modifiedRecord;
        }
        /// <summary>
        /// 恢复nuget引用
        /// </summary>
        public void RevertNuget()
        {
            var rootElement = Document.Root;
            if (rootElement == null)
            {
                throw new InvalidOperationException($"document.Root是空的,{XmlFile}");
            }
            var packageElements = rootElement.Elements().ToList();
            var packageElement = new XElement("package");
            packageElement.SetAttributeValue("id", _replacedFileRecord.NugetName);
            packageElement.SetAttributeValue("version", _replacedFileRecord.Version);
            packageElement.SetAttributeValue("targetFramework", _replacedFileRecord.TargetFramework);

            //在之前位置插入Reference引用
            if (_replacedFileRecord.ModifiedLineIndex >= packageElements.Count)
            {
                packageElements[packageElements.Count - 1].AddAfterSelf(packageElement);
            }
            else
            {
                packageElements[_replacedFileRecord.ModifiedLineIndex].AddBeforeSelf(packageElement);
            }
            SaveFile();
        }
    }
}
