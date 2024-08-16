using System.Xml.Linq;

namespace NugetEfficientTool.Nuget
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
            if (root.Name.LocalName != CsProjConst.RootName)
            {
                ExceptionMessage = $".csproj 文件根节点名称不为 ${CsProjConst.RootName}";
                return false;
            }

            foreach (var packageReference in CsProj.GetNugetReferences(_xDocument))
            {
                if (packageReference.Attribute(CsProjConst.IncludeAttribute) == null)
                {
                    ExceptionMessage = $"{CsProjConst.PackageReferenceName} 缺少 {CsProjConst.IncludeAttribute} 属性。";
                    return false;
                }

                var nugetInfo = CsProj.GetNugetInfo(packageReference);
                if (nugetInfo == null)
                {
                    ExceptionMessage = $"{packageReference.Name}，不包含nuget信息。{packageReference.Value}。";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(nugetInfo.Name))
                {
                    ExceptionMessage = $"{packageReference.Name}，不包含nuget名称。";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(nugetInfo.Version))
                {
                    ExceptionMessage = $"{packageReference.Name}，版本信息缺失。";
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
            var references = CsProj.GetNugetReferences(_xDocument);
            foreach (var reference in references)
            {
                var nugetInfo = CsProj.GetNugetInfo(reference, _csProjPath);
                if (nugetInfo == null || string.IsNullOrWhiteSpace(nugetInfo.Name) || string.IsNullOrWhiteSpace(nugetInfo.Version))
                {
                    continue;
                }
                nugetInfoList.Add(nugetInfo);
            }
            //组件项目，添加自己的版本
            if (CsProj.IsComponent(_xDocument))
            {
                var componentVersion = CsProj.GetComponentVersion(_xDocument);
                if (!string.IsNullOrEmpty(componentVersion))
                {
                    var componentName = Path.GetFileNameWithoutExtension(_csProjPath);
                    nugetInfoList.Add(new NugetInfo(componentName, componentVersion));
                }
            }
            return nugetInfoList;
        }

        #region 私有变量

        private readonly XDocument _xDocument;

        private readonly string _csProjPath;

        private bool? _isGoodFormat;

        #endregion
    }
}