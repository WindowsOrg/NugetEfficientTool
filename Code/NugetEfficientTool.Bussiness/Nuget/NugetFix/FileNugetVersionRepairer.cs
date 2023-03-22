using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget文件修复器
    /// </summary>
    public class FileNugetVersionRepairer
    {
        #region 构造函数

        /// <summary>
        /// 构造一个 Nuget 配置文件修复器
        /// </summary>
        /// <param name="configPath">Nuget 配置文件路径</param>
        /// <param name="nugetFixStrategies">修复策略</param>
        public FileNugetVersionRepairer(string configPath, IEnumerable<NugetFixStrategy> nugetFixStrategies)
        {
            if (nugetFixStrategies == null)
            {
                throw new ArgumentNullException(nameof(nugetFixStrategies));
            }
            _xDocument = new XmlReader(configPath).Document;
            _configPath = configPath;
            switch (NugetConfig.GetNugetConfigType(configPath))
            {
                case NugetConfigType.PackagesConfig:
                    _nugetConfigFixer = new PackagesReferenceFixer(_xDocument, configPath, nugetFixStrategies);
                    break;
                case NugetConfigType.CsProj:
                    _nugetConfigFixer = new CsProjReferenceFixer(_xDocument, configPath, nugetFixStrategies);
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
                _xDocument = _nugetConfigFixer.Fix();
                if (_nugetConfigFixer.SucceedStrategies.Any())
                {
                    if (_nugetConfigFixer.Log == null || string.IsNullOrEmpty(_nugetConfigFixer.Log))
                    {
                        var fixedNugets = string.Join(",", _nugetConfigFixer.SucceedStrategies.Select(i => $"{i.NugetName}{i.NugetVersion}"));
                        Log = $"对 {_configPath} 执行{fixedNugets}{CustomText.FixErrorKey}";
                    }
                    else
                    {
                        var headerMessage = string.Format(CustomText.FixSuccessKey,_configPath);
                        Log = StringSplicer.SpliceWithNewLine(headerMessage, _nugetConfigFixer.Log);
                    }
                }
                _xDocument.Save(_configPath);
                return true;
            }
            catch (Exception e)
            {
                Log = StringSplicer.SpliceWithNewLine($"{CustomText.FixErrorKey}，{e.Message}", e.StackTrace);
                return false;
            }
        }

        #endregion

        #region 内部变量

        private readonly string _configPath;

        private XDocument _xDocument;

        private readonly NugetReferenceFixer _nugetConfigFixer;

        #endregion
    }
}