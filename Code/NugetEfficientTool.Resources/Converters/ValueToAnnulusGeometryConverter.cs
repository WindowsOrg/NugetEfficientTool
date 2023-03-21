using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Resources
{
    internal class ValueToAnnulusGeometryConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //传入的三个value依次为：Value，Minimum，Maximum
            double angle = 0;
            if (values.All(x => x is double) && values.Length >= 3)
            {
                var range = (double)values[2] - (double)values[1];
                if (range <= 0)
                {
                    angle = 0;
                }
                else
                {
                    angle = (double)values[0] * Math.PI * 2 / range;
                }
            }

            //传入多个参数，用 | 隔开，circle代表圆心（x==y，故只用一个数表示，radius表示半径）
            var parameters = ((string)parameter).Split('|');
            var circle = double.Parse(parameters[0]);
            var radius = double.Parse(parameters[1]);

            var point = CirclePointUtil.GetPointByAngel(new Point(circle, circle), radius, angle);
            string miniLang;
            if (angle <= Math.PI)
            {
                miniLang = $"M{circle},{circle - radius} A{radius},{radius} 0 0 1 {point.X},{point.Y}";
            }
            else
            {
                miniLang = $"M{circle},{circle - radius} A{radius},{radius} 0 0 1 {circle},{circle + radius} A{radius},{radius} 0 0 1 {point.X},{point.Y}";
            }
            return Geometry.Parse(miniLang);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
