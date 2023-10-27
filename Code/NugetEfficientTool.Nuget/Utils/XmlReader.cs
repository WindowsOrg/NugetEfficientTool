using System.Xml;
using System.Xml.Linq;

namespace Kybs0.Csproj.Analyzer
{
    /// <summary>
    /// XML 读取器
    /// </summary>
    public class XmlReader
    {
        #region 构造函数

        /// <summary>
        /// 构造一个 XML 读取器
        /// </summary>
        /// <param name="filePath">XML 文件路径</param>
        public XmlReader( string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            FilePath = filePath;
            LoadXml();
        }

        #endregion

        #region 私有变量

        protected bool? IsGoodFormatAtLastCheck;

        #endregion

        #region 公共字段

        /// <summary>
        /// XML 文件路径
        /// </summary>
        
        public string FilePath { get; }

        /// <summary>
        /// XML 文档
        /// </summary>
        public XDocument Document { get; private set; }

        /// <summary>
        /// 根节点
        /// </summary>
        public XElement Root => Document?.Root;

        /// <summary>
        /// 文件错误信息
        /// </summary>
        
        public string ErrorMessage { get; protected set; } = string.Empty;

        #endregion

        #region 公共方法

        /// <summary>
        /// 检查是否格式正常
        /// </summary>
        /// <returns>是否格式正常</returns>
        public bool IsGoodFormat()
        {
            if (IsGoodFormatAtLastCheck.HasValue)
            {
                return IsGoodFormatAtLastCheck.Value;
            }

            IsGoodFormatAtLastCheck = CheckFormat();
            return IsGoodFormatAtLastCheck.Value;
        }

        /// <summary>
        /// 尝试修复 XML 文件中存在的错误
        /// </summary>
        /// <returns>返回修复成功与否，若无需修复，也返回成功</returns>
        public virtual bool TryToFix()
        {
            // 此处暂不提供修复，直接返回当前文件格式
            return IsGoodFormat();
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 检查文件格式是否正常
        /// </summary>
        /// <returns>检查结果</returns>
        protected virtual bool CheckFormat()
        {
            return string.IsNullOrWhiteSpace(ErrorMessage);
        }

        /// <summary>
        /// 从 XML 中解析更多数据
        /// </summary>
        protected virtual void ParseXml()
        {
        }

        /// <summary>
        /// 载入 XML 文件并获取内容
        /// </summary>
        private void LoadXml()
        {
            try
            {
                Document = XDocument.Load(FilePath);
                if (CheckFormat())
                {
                    ParseXml();
                }
            }
            catch (XmlException xmlException)
            {
                Console.WriteLine($@"{FilePath} 检测到格式异常。");
                Console.WriteLine($@"异常原因：{xmlException.Message}");
                ErrorMessage = $"{FilePath} 存在格式异常：{Environment.NewLine}  {xmlException.Message}";
            }
        }

        #endregion
    }
}