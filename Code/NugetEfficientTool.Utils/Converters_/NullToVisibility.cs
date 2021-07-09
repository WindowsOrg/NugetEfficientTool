using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NugetEfficientTool.Utils
{
    public class NullToVisibility:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null||value is string stringValue&&string.IsNullOrEmpty(stringValue) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
