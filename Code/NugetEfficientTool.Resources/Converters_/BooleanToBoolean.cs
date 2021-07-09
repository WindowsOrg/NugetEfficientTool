using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NugetEfficientTool.Resources
{
    public static class BooleanToBoolean
    {
        public static IValueConverter ReverseBoolean { get; } = new ReverseBooleanConverter();
    }

    /// <summary>
    /// boolean反转转换器
    /// </summary>
    public class ReverseBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return default(bool);
            }
            var v = (bool)value;
            return !v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return default(bool);
            }
            var v = (bool)value;
            return !v;
        }
    }
}
