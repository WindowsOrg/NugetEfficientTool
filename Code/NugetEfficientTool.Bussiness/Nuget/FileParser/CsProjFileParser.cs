using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget 解析器
    /// 用于解析 csproj 文件中的 Nuget 信息
    /// </summary>
    internal class CsProjFileParser : INugetFileParser
    {
        public CsProjFileParser(XDocument xDocument, string csProjPath)
        {
            _xDocument = xDocument;
            _csProjPath = csProjPath;
        }

        /// <summary>
        /// 解析时的异常信息
        /// </summary>
        public string ExceptionMessage { get; private set; }

        /// <summary>
        /// 是否格式正常
        /// </summary>
        /// <returns>是否格式正常</returns>
        public bool IsGoodFormat()
        {
            if (_isGoodFormat.HasValue)
            {
                return _isGoodFormat.Value;
            }

            _isGoodFormat = false;
            var root = _xDocument.Root;
            if (root.Name.LocalName != CsProj.RootName)
            {
                ExceptionMessage = $".csproj 文件根节点名称不为 ${CsProj.RootName}";
                return false;
            }

            foreach (var packageReference in CsProj.GetPackageReferences(_xDocument))
            {
                if (packageReference.Attribute(CsProj.IncludeAttribute) == null &&
                    packageReference.Attribute(CsProj.UpdateAttribute) == null)
                {
                    ExceptionMessage = $"{CsProj.PackageReferenceName} 缺少 {CsProj.IncludeAttribute} 属性。";
                    return false;
                }

                if (packageReference.Attribute(CsProj.VersionAttribute) == null
                    && packageReference.Elements().FirstOrDefault(x => x.Name.LocalName == CsProj.VersionElementName) ==
                    null)
                {
                    ExceptionMessage = $"{CsProj.PackageReferenceName} 缺少必要的版本信息。";
                    return false;
                }
            }

            _isGoodFormat = true;
            return true;
        }

        /// <summary>
        /// 获取 Nuget 包信息列表
        /// </summary>
        /// <returns>Nuget 包信息列表</returns>
        public IEnumerable<NugetInfo> GetNugetInfos()
        {
            if (!IsGoodFormat())
            {
                throw new InvalidOperationException("无法在格式异常的配置文件中读取 Nuget 信息。");
            }

            var nugetInfoList = new List<NugetInfo>();
            var packageReferences = CsProj.GetPackageReferences(_xDocument);
            foreach (var packageReference in packageReferences)
            {
                var nugetName = GetNugetName(packageReference);
                var nugetVersion = GetNugetVersion(packageReference);
                if (string.IsNullOrWhiteSpace(nugetName) || string.IsNullOrWhiteSpace(nugetVersion))
                {
                    continue;
                }

                nugetInfoList.Add(new NugetInfo(nugetName, nugetVersion));
            }

            foreach (var nugetInfoReference in CsProj.GetNugetInfoReferences(_xDocument))
            {
                var nugetInfo = CsProj.GetNugetInfoFromNugetInfoReference(nugetInfoReference, _csProjPath);
                nugetInfoList.Add(nugetInfo);
            }

            return nugetInfoList;
        }

        private string GetNugetName(XElement xElement)
        {
            var includeAttribute = xElement.Attribute(CsProj.IncludeAttribute);
            if (includeAttribute != null)
            {
                return includeAttribute.Value;
            }

            var updateAttribute = xElement.Attribute(CsProj.UpdateAttribute);
            if (updateAttribute != null)
            {
                return updateAttribute.Value;
            }

            ShowExceptionMessageBox(xElement);
            return string.Empty;
        }

        private string GetNugetVersion(XElement xElement)
        {
            var versionAttribute = xElement.Attribute(CsProj.VersionAttribute);
            if (versionAttribute != null)
            {
                return versionAttribute.Value;
            }

            var childElements = xElement.Elements();
            var firstVersionAttribute = childElements.FirstOrDefault(x => x.Name.LocalName == CsProj.VersionAttribute);
            if (firstVersionAttribute == null)
            {
                ShowExceptionMessageBox(xElement);
                return string.Empty;
            }

            return firstVersionAttribute.Value;
        }

        private void ShowExceptionMessageBox(XElement xElement)
        {
            throw new ArgumentException($"发现一个无法解析的键，请保留现场，联系开发者。{Environment.NewLine}{xElement}");
        }

        #region 私有变量

        private readonly XDocument _xDocument;

        private readonly string _csProjPath;

        private bool? _isGoodFormat;

        #endregion
    }
}