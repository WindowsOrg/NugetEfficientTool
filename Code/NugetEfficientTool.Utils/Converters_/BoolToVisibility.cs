using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NugetEfficientTool.Utils
{
    public class BoolToVisibility:IValueConverter
    {
        public bool IsReverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Collapsed;
            if (value is bool boolValue && boolValue)
            {
                visibility= Visibility.Visible;
            }

            return IsReverse?(visibility== Visibility.Visible?Visibility.Collapsed:Visibility.Visible): (visibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
