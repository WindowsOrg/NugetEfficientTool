using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NugetEfficientTool.Resources
{
    public class VisibilityReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
