using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举/中文字典
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnumDict<TEnum>(this TEnum enumValue) where TEnum :
            struct
        {
            Type type = enumValue.GetType();

            Dictionary<string, string> dict = new Dictionary<string, string>();
            FieldInfo[] fields = type.GetFields();

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    var customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false).ToList();
                    dict.Add(field.Name, ((DescriptionAttribute)customAttributes[0]).Description);
                }
            }

            return dict;
        }
        /// <summary>
        /// 获取枚举描述特性值
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue">枚举值</param>
        /// <returns>枚举值的描述</returns>
        public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum :
            struct
        {
            Type type = enumValue.GetType();
            //枚举的成员信息
            foreach (var memberInfo in type.GetMembers())
            {
                if (memberInfo.Name != enumValue.ToString()) continue;

                //获取自定义标记
                foreach (Attribute attr in memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false))
                {
                    var attribute = attr as DescriptionAttribute;

                    if (attribute == null) continue;

                    return attribute.Description;
                }
            }

            return string.Empty;
        }
    }
}
