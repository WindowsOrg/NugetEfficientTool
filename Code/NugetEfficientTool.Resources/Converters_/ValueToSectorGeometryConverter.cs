using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// 角度转扇形路径的转换器
    /// </summary>
    internal class ValueToSectorGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //此段代码用来缓解进度条抖动问题
            var currentValue = System.Convert.ToInt32(value);
            if ((currentValue) % 3 != 0) currentValue = (currentValue / 3) * 3;

            var angle = (double)currentValue * Math.PI * 2 / 100;
            var point = CirclePointUtil.GetPointByAngel(new Point(50, 50), 50, angle);
            string miniLang;
            if (angle <= Math.PI)
            {
                miniLang = string.Format(culture, "M50,50 L50,0 A50,50 0 0 1 {0},{1} L50,50Z", point.X, point.Y);
            }
            else
            {
                miniLang = string.Format(culture, "M50,50 L50,0 A50,50 0 0 1 50,100 A50,50 0 0 1 {0},{1}Z", point.X, point.Y);
            }
            return Geometry.Parse(miniLang);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}
