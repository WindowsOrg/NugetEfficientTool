using System;
using System.Collections.Generic;
using System.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget 读取器
    /// </summary>
    public class NugetConfigReader : XmlReader
    {
        #region 构造函数

        /// <summary>
        /// 构造一个 Nuget 配置文件读取器
        /// </summary>
        /// <param name="filePath">Nuget 配置文件路径</param>
        public NugetConfigReader( string filePath) : base(filePath)
        {
        }

        #endregion

        #region 公共字段

        /// <summary>
        /// Nuget 包信息列表
        /// </summary>
        public IEnumerable<FileNugetInfo> PackageInfoExs { get; private set; }

        #endregion

        #region 私有变量

        private INugetFileParser _nugetConfigParser;

        #endregion

        #region 内部方法

        /// <summary>
        /// 检查文件格式是否正常
        /// </summary>
        /// <returns>检查结果</returns>
        protected override bool CheckFormat()
        {
            if (!base.CheckFormat())
            {
                return false;
            }

            switch (NugetConfig.GetNugetConfigType(FilePath))
            {
                case NugetConfigType.PackagesConfig:
                    _nugetConfigParser = new PackagesFileParser(Document);
                    break;
                case NugetConfigType.CsProj:
                    _nugetConfigParser = new CsProjFileParser(Document, FilePath);
                    break;
                case NugetConfigType.Unknown:
                    ErrorMessage = $"无法判断 {FilePath} 是哪种类型的 Nuget 配置文件";
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!_nugetConfigParser.IsGoodFormat())
            {
                ErrorMessage = CreateFormatErrorMessage(_nugetConfigParser.ExceptionMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 从 XML 中解析更多数据
        /// </summary>
        protected override void ParseXml()
        {
            var nugetInfos = _nugetConfigParser.GetNugetInfos();
            var nugetInfoExs = nugetInfos.Select(x => new FileNugetInfo(x, FilePath));
            PackageInfoExs = nugetInfoExs;
        }

        private string CreateFormatErrorMessage(string errorMessage)
        {
            return $"{FilePath}{Environment.NewLine}  {errorMessage}";
        }

        #endregion
    }
}