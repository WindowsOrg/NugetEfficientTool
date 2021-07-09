using System;
using System.Text.RegularExpressions;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 排序
    /// 由0-9 a-z A-Z
    /// </summary>
    public class SortingString : IComparable, IComparable<SortingString>
    {
        /// <summary>排序的内容</summary>
        private string Content { get; set; }

        /// <summary>创建排序字符串</summary>
        /// <param name="content"></param>
        public SortingString(string content)
        {
            Content = content;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Content;
        }

        /// <summary>
        /// 排序规则：
        /// 1.全数字 由小到大
        /// 2.混合，特殊字符0-9-a-z-A-Z(汉字按拼音首字母排序） 注：如有数字 由小到大。
        /// 优化级：数字-字母-汉字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (this == obj)
                return 0;
            if (obj == null)
                return 1;
            return CompareTo((SortingString) obj);
        }

        /// <inheritdoc />
        public int CompareTo(SortingString other)
        {
            if (this == other)
                return 0;
            if (other == null)
                return 1;
            string content1 = GetContent(Content);
            string content2 = GetContent(other.Content);
            int num = NumberCompareHelper.CompareNumber(content1, content2);
            if (num != 0)
                return num;
            return string.Compare(content1, content2, StringComparison.CurrentCulture);
        }

        /// <summary>去除特殊字符</summary>
        protected virtual string GetContent(string content)
        {
            return content;
        }
    }
    /// <summary>
    /// 数字比较
    /// </summary>
    internal static class NumberCompareHelper
    {
        public static int CompareNumber(string content, string otherContent)
        {
            string pattern1 = "\\d+";
            //判断是否含有数字
            if (TryGetDouble(content, out _, pattern1) && TryGetDouble(otherContent, out _, pattern1))
            {
                int num2 = content.Length > otherContent.Length ? otherContent.Length : content.Length;
                string pattern2 = "^-?\\d+(\\.\\d+)?";
                for (int startIndex = 0; startIndex < num2; ++startIndex)
                {
                    string content1 = content.Substring(startIndex, content.Length - startIndex);
                    string content2 = otherContent.Substring(startIndex, otherContent.Length - startIndex);
                    if (TryGetDouble(content1, out double num3, pattern2) && TryGetDouble(content2, out double num4, pattern2))
                    {
                        if (num3 > num4)
                            return 1;
                        if (num3 < num4)
                            return -1;
                    }
                    else if (content[startIndex] != otherContent[startIndex])
                        return 0;
                }
            }
            return 0;
        }

        /// <summary>尝试转换名称为数字</summary>
        private static bool TryGetDouble(string content, out double num, string pattern)
        {
            num = 0.0;
            MatchCollection matchCollection = Regex.Matches(content, pattern);
            return matchCollection.Count > 0 && matchCollection[0].Success && double.TryParse(matchCollection[0].Value, out num);
        }
    }
}
