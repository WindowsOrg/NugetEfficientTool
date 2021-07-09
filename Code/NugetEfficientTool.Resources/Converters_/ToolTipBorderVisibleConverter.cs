using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NugetEfficientTool.Resources
{
    public class ToolTipBorderVisibleConverter : IValueConverter
    {
        public static readonly ToolTipBorderVisibleConverter Instance = new ToolTipBorderVisibleConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //check if the ToolTip's content property is null or empty        
            if (value == null || value.ToString() == string.Empty)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
