using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NugetEfficientTool.Resources;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 弹框
    /// </summary>
    public class NotificationWindowProvider : INotificationWindowProvider
    {
        public INotificationWindow GetTipWindow()
        {
            return new NotificationTipWindow();
        }
        public INotificationWindow GetInfoWindow()
        {
            return new NotificationWindow();
        }
    }

    public interface INotificationWindowProvider
    {
        INotificationWindow GetTipWindow();
        INotificationWindow GetInfoWindow();
    }
}
