using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    public class NugetConfigRepairer
    {
        #region 构造函数

        /// <summary>
        /// 构造一个 Nuget 配置文件修复器
        /// </summary>
        /// <param name="configPath">Nuget 配置文件路径</param>
        public NugetConfigRepairer( string configPath,
             IEnumerable<NugetFixStrategy> nugetFixStrategies)
        {
            _nugetFixStrategies = nugetFixStrategies ?? throw new ArgumentNullException(nameof(nugetFixStrategies));
            _xDocument = new XmlReader(configPath).Document;
            _configPath = configPath;
            switch (NugetConfig.GetNugetConfigType(configPath))
            {
                case NugetConfigType.PackagesConfig:
                    _nugetConfigFixHelper = new PackagesConfigFixHelper(_xDocument, _nugetFixStrategies);
                    break;
                case NugetConfigType.CsProj:
                    _nugetConfigFixHelper = new CsProjFixHelper(_xDocument, configPath, _nugetFixStrategies);
                    break;
                case NugetConfigType.Unknown:
                    Log = $"无法判断 {configPath} 是哪种类型的 Nuget 配置文件";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region 公共字段

        /// <summary>
        /// 修复日志
        /// </summary>
        public string Log { get; private set; } = string.Empty;

        #endregion

        #region 公共方法

        /// <summary>
        /// 执行修复
        /// </summary>
        /// <returns>是否修复成功</returns>
        public bool Repair()
        {
            try
            {
                _xDocument = _nugetConfigFixHelper.Fix();
                var headerMessage = $"对 {_configPath} 执行了以下修复操作：";
                Log = StringSplicer.SpliceWithNewLine(headerMessage, _nugetConfigFixHelper.Log);
                _xDocument.Save(_configPath);
                return true;
            }
            catch (Exception e)
            {
                Log = StringSplicer.SpliceWithNewLine(e.Message, e.StackTrace);
                return false;
            }
        }

        #endregion

        #region 内部变量

        private readonly string _configPath;

        private XDocument _xDocument;

        private readonly INugetConfigFixHelper _nugetConfigFixHelper;

        private readonly IEnumerable<NugetFixStrategy> _nugetFixStrategies;

        #endregion
    }
}