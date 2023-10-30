using System.Xml.Linq;

namespace Kybs0.Project
{
    /// <summary>
    /// package.config文件辅助类
    /// </summary>
    public static class PackageFiles
    {
        /// <summary>
        /// 获取 Nuget 包信息列表
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static IEnumerable<NugetInfo> GetNugetInfos(XDocument xDocument)
        {
           return new PackagesFileParser(xDocument).GetNugetInfos();
        }
    }
}
