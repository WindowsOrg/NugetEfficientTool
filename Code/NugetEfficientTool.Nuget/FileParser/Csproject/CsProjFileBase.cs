using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Kybs0.Project
{
    internal abstract class CsProjFileBase
    {
        /// <summary>
        /// 获取ProjectReference节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public List<XElement> GetProjectReferences(XDocument xDocument)
        {
            return GetXElementsByNameInItemGroups(xDocument, CsProjConst.ProjectReferenceName);
        }

        /// <summary>
        /// 获取Element名称对应的节点
        /// </summary>
        /// <param name="xDocument"></param>
        /// <param name="xElementName"></param>
        /// <returns></returns>
        protected List<XElement> GetXElementsByNameInItemGroups(XDocument xDocument,
            string xElementName)
        {
            if (xDocument == null)
            {
                throw new ArgumentNullException(nameof(xDocument));
            }

            if (xElementName == null)
            {
                throw new ArgumentNullException(nameof(xElementName));
            }

            var xElementList = new List<XElement>();
            var itemGroupElements = xDocument.Root.Elements().Where(x => x.Name.LocalName == CsProjConst.ItemGroupName);
            foreach (var itemGroupElement in itemGroupElements)
            {
                xElementList.AddRange(itemGroupElement.Elements().Where(x => x.Name.LocalName == xElementName));
            }

            return xElementList;
        }

        protected static readonly Regex IncludeValueRegex = new Regex(@".+,\s*Version=.+,\s*Culture=.+,\s*processorArchitecture=.+");

    }
}
