using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 资源字典辅助类
    /// </summary>
    public class ResourceDictionaryExtensions
    {
        private const string KeyConstString = "x:Key=\"";
        /// <summary>
        /// 从文件获取所有资源值
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IEnumerable<SingleResource> GetAllKeysFromFile(string filePath)
        {
            var result = new List<SingleResource>();
            var list = File.ReadAllLines(filePath).ToList();

            SingleResource singleResource = null;
            foreach (var textLine in list)
            {
                if (textLine.Contains(KeyConstString))
                {
                    var startIndex = textLine.IndexOf(KeyConstString) + KeyConstString.Length;
                    var endIndex = textLine.IndexOf("\"", startIndex);
                    var keyString = textLine.Substring(startIndex, endIndex - startIndex);

                    singleResource = new SingleResource();
                    singleResource.Key = keyString;
                    singleResource.LineIndex = list.IndexOf(textLine);
                    singleResource.ResourceLines.Add(textLine);
                    result.Add(singleResource);
                }
                else
                {
                    singleResource?.ResourceLines.Add(textLine);
                }
            }
            return result;
        }
    }
    public class SingleResource
    {
        public int LineIndex { get; set; }
        public string Key { get; set; }
        public List<string> ResourceLines { get; set; } = new List<string>();
    }
}
