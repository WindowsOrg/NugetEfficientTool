using System;
using System.Globalization;
using System.Windows.Data;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Resources
{
    class DownloadStateToProgressStateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DownloadState downloadState)
            {
                return downloadState.ToProgressState();
            }

            return ProgressState.Initial;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
