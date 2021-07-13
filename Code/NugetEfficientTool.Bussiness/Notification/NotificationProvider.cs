using System.Threading.Tasks;
using System.Windows;
using NugetEfficientTool.Resources;

namespace NugetEfficientTool.Business
{
    public class NotificationProvider : INotificationProvider
    {
        public void ShowInfo(Window owner, string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var infoWindow = _notificationWindowProvider.GetTipWindow();
                infoWindow.SetText(text);
                var ownerWindow = owner;
                if (ownerWindow != null)
                {
                    infoWindow.SetOwner(ownerWindow);
                }

                infoWindow.Show();
                if (ownerWindow != null)
                {
                    ((Window)infoWindow).Top = ownerWindow.Top + 40;
                }
            });
        }

        public async Task<bool?> ShowAsync(string text)
        {
            return await ShowAsync(null, text);
        }

        public async Task<bool?> ShowAsync(Window owner, string text, string title = "")
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
             {
                 var infoWindow = _notificationWindowProvider.GetInfoWindow();
                 infoWindow.SetText(text,title);
                 var ownerWindow = owner;
                 if (ownerWindow != null)
                 {
                     infoWindow.SetOwner(ownerWindow);
                 }

                 var result = infoWindow.ShowDialog();
                 return result;
             });
        }

        public async Task<bool?> ShowAsync(Window owner, string text,string subText, string title = "")
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var infoWindow = _notificationWindowProvider.GetInfoWindow();
                infoWindow.SetText(text, subText, title);
                var ownerWindow = owner;
                if (ownerWindow != null)
                {
                    infoWindow.SetOwner(ownerWindow);
                }

                var result = infoWindow.ShowDialog();
                return result;
            });
        }

        public async Task<bool?> ShowAsync(UIElement uiElement)
        {
            return await ShowAsync(null, uiElement);
        }

        public async Task<bool?> ShowAsync(Window owner, UIElement uiElement, string title = "")
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var infoWindow = _notificationWindowProvider.GetInfoWindow();
                infoWindow.SetView(uiElement);
                var ownerWindow = owner;
                if (ownerWindow != null)
                {
                    infoWindow.SetOwner(ownerWindow);
                }
                var result = infoWindow.ShowDialog();
                return result;
            });
        }

        public async Task<bool?> AlertAsync(string text)
        {
            return await AlertAsync(null, text);
        }

        public async Task<bool?> AlertAsync(Window owner, string text, string title="")
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var infoWindow = _notificationWindowProvider.GetInfoWindow();
                infoWindow.SetNotificationContent(new NotificationContent(text,title,owner)
                {
                    CancelButton = new NotificationButtonInfo(){Content = "取消"}
                });
                //owner
                var ownerWindow = owner;
                if (ownerWindow != null)
                {
                    infoWindow.SetOwner(ownerWindow);
                }

                var result = infoWindow.ShowDialog();
                return result;
            });
        }

        public async Task<bool?> AlertAsync(UIElement uiElement)
        {
           return await AlertAsync(null,uiElement);
        }

        public async Task<bool?> AlertAsync(Window owner, UIElement uiElement, string title="")
        {
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var infoWindow = _notificationWindowProvider.GetInfoWindow();
                infoWindow.SetNotificationContent(new NotificationContent(uiElement,title,owner)
                {
                    CancelButton = new NotificationButtonInfo(){Content = "取消"}
                });
                var ownerWindow = owner;
                if (ownerWindow != null)
                {
                    infoWindow.SetOwner(ownerWindow);
                }
                var result = infoWindow.ShowDialog();
                return result;
            });
        }


        private readonly INotificationWindowProvider _notificationWindowProvider = new NotificationWindowProvider();
    }

    public interface INotificationProvider
    {
        void ShowInfo(Window owner, string text);
        Task<bool?> ShowAsync(Window owner, string text, string title = "");
        Task<bool?> ShowAsync(Window owner, string text,string subText, string title = "");
        Task<bool?> ShowAsync(UIElement uiElement);
        Task<bool?> ShowAsync(Window owner, UIElement uiElement, string title = "");
        Task<bool?> AlertAsync(Window owner, string text, string title = "");
        Task<bool?> AlertAsync(UIElement uiElement);
        Task<bool?> AlertAsync(Window owner, UIElement uiElement, string title = "");
    }
}
