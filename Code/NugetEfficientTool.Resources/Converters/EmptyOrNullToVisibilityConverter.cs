using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NugetEfficientTool.Resources
{
    public class EmptyOrNullToVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is string stringValue && string.IsNullOrEmpty(stringValue))
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
