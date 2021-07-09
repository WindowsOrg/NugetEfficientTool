using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 单位换算辅助类
    /// </summary>
    public class UnitConvertHelper
    {

        /// <summary>
        /// 转化为16进制
        /// </summary>
        /// <param name="intValue"></param>
        /// <param name="returnedValueMinLength"></param>
        /// <returns></returns>
        public static string ConvertToUnit16(int intValue, int returnedValueMinLength)
        {
            var unit16Value = $"{intValue:X}";
            unit16Value = unit16Value.PadLeft(returnedValueMinLength, '0');
            return unit16Value;
        }

        /// <summary>
        /// 转化为
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static string ConvertToUnit16(int intValue)
        {
            var unit16Value = ConvertToUnit16(intValue, 4);
            return unit16Value;
        }
        /// <summary>
        /// 将32位数字转换为高低位字节位数字
        /// </summary>
        /// <param name="intValue">待转化的数字</param>
        /// <returns></returns>
        internal static Tuple<byte, byte> ConvertToByteUnit(int intValue)
        {
            try
            {
                var hightIntValue = intValue / 256;
                var hightStringValue = hightIntValue.ToString().PadLeft(4, '0');
                //byte_Length = System.Text.Encoding.UTF8.GetBytes(SendLength.ToString().PadLeft(4,, '0'));
                var highByteValue = Convert.ToByte(intValue / 256);
                var lowByteValue = Convert.ToByte(intValue % 256);
                return Tuple.Create(highByteValue, lowByteValue);
            }
            catch (Exception)
            {
                return Tuple.Create(Convert.ToByte(0), Convert.ToByte(0));
            }
        }
    }
}
