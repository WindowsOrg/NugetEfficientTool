using System;
using System.Globalization;
using System.Windows.Data;

namespace NugetEfficientTool.Utils
{
    public class StringToBoolConveter:IValueConverter
    {
        public bool IsReverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                var boolValue = !string.IsNullOrEmpty(stringValue);
                return IsReverse?!boolValue:boolValue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
