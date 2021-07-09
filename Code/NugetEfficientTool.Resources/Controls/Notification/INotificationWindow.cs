using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetEfficientTool.Resources
{
    public partial interface INotificationWindow
    {
        void SetText(string text, string title = "");
        void SetText(string text, string subText, string title = "");
        void SetView(UIElement view, string title = "");
        void SetOwner(Window window);
        void SetNotificationContent(NotificationContent content);
        void Show();
        bool? ShowDialog();
    }

    public class NotificationContent
    {
        public NotificationContent(string content, string title = "", Window owner = null)
        {
            Title = title;
            Content = content;
            Owner = owner;
        }
        public NotificationContent(UIElement view, string title = "", Window owner = null)
        {
            Title = title;
            View = view;
            Owner = owner;
        }
        public string Title { get; }
        public string Content { get; }
        public UIElement View { get; }
        public Window Owner { get; }
        public NotificationButtonInfo OkButton { get; set; }
        public NotificationButtonInfo CancelButton { get; set; }
    }

    public class NotificationButtonInfo
    {
        public string Content { get; set; }
    }
}
