using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Size = System.Drawing.Size;

namespace NugetEfficientTool.Utils
{
    public class PopupHelper
    {
        #region Popup位置随窗口位置移动而变化

        public static DependencyObject GetLocationUpdatedOnTarget(DependencyObject obj)
        {
            return (DependencyObject)obj.GetValue(LocationUpdatedOnTargetProperty);
        }

        public static void SetLocationUpdatedOnTarget(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(LocationUpdatedOnTargetProperty, value);
        }

        /// <summary>
        /// Popup位置更新
        /// </summary>
        public static readonly DependencyProperty LocationUpdatedOnTargetProperty =
            DependencyProperty.RegisterAttached("LocationUpdatedOnTarget", typeof(DependencyObject), typeof(PopupHelper), new PropertyMetadata(null, OnPopupPlacementTargetChanged));

        private static void OnPopupPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Popup pop = d as Popup;

            //旧值取消LocationChanged监听
            if (e.OldValue is DependencyObject previousPlacementTarget)
            {
                Window window = Window.GetWindow(previousPlacementTarget);
                if (window != null)
                {
                    window.LocationChanged -= WindowLocationChanged;
                }
            }

            //新值添加LocationChanged监听
            if (e.NewValue is DependencyObject newPlacementTarget)
            {
                //如果窗口变动，popup位置更新
                var window = Window.GetWindow(newPlacementTarget);
                if (window != null)
                {
                    AddEvent();
                }
                else if (newPlacementTarget is FrameworkElement frameworkElement)
                {
                    frameworkElement.Loaded -= FrameworkElementOnLoaded;
                    frameworkElement.Loaded += FrameworkElementOnLoaded;
                    void FrameworkElementOnLoaded(object sender, RoutedEventArgs args)
                    {
                        frameworkElement.Loaded -= FrameworkElementOnLoaded;
                        window = Window.GetWindow(newPlacementTarget);
                        if (window != null)
                        {
                            AddEvent();
                        }
                    }
                }

                void AddEvent()
                {
                    window.LocationChanged -= WindowLocationChanged;
                    window.LocationChanged += WindowLocationChanged;
                    window.SizeChanged -= WinOnSizeChanged;
                    window.SizeChanged += WinOnSizeChanged;
                }
            }
            void WindowLocationChanged(object s1, EventArgs e1)
            {
                UpdatePopupPosition(pop);
            }
            void WinOnSizeChanged(object sender, SizeChangedEventArgs args)
            {
                UpdatePopupPosition(pop);
            }
        }

        /// <summary>
        /// 强制刷新Pop位置
        /// </summary>
        /// <param name="pop"></param>
        public static void UpdatePopupPosition(Popup pop)
        {
            if (pop != null && pop.IsOpen)
            {
                try
                {
                    //通知更新相对位置
                    var updatePositionMethodInfo = typeof(Popup).GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
                    updatePositionMethodInfo?.Invoke(pop, null);
                }
                catch (Exception)
                {
                    //忽略异常,popup未强制刷新位置不应该影响流程
                }
            }
        }

        #endregion

        #region 设置Popup只在当前窗口顶层

        #region 附加属性-TopmostInCurrentWindow

        private static readonly DependencyProperty TopmostInCurrentWindowProperty = DependencyProperty.RegisterAttached("TopmostInCurrentWindow",
            typeof(bool), typeof(PopupHelper), new FrameworkPropertyMetadata(false, OnTopmostInCurrentWindowChanged));

        public static bool GetTopmostInCurrentWindow(DependencyObject obj)
        {
            return (bool)obj.GetValue(TopmostInCurrentWindowProperty);
        }

        public static void SetTopmostInCurrentWindow(DependencyObject obj, bool value)
        {
            obj.SetValue(TopmostInCurrentWindowProperty, value);
        }

        private static void OnTopmostInCurrentWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Popup popup)
            {
                popup.Opened -= PopupOnOpened;
                popup.Closed -= PopupOnClosed;
                if ((bool)e.NewValue)
                {
                    popup.Opened += PopupOnOpened;
                    popup.Closed += PopupOnClosed;
                }
            }
        }

        #endregion

        #region 事件监听

        private static void PopupOnOpened(object sender, EventArgs e)
        {
            if (!(sender is Popup popup))
                return;

            popup.Child?.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown), true);

            var parentWindow = Window.GetWindow(popup);
            if (parentWindow == null)
                return;
            if (!Popups.ContainsKey(popup))
            {
                Popups.Add(popup, parentWindow);
            }
            parentWindow.Activated -= OnParentWindowActivated;
            parentWindow.Deactivated -= OnParentWindowDeactivated;
            parentWindow.Activated += OnParentWindowActivated;
            parentWindow.Deactivated += OnParentWindowDeactivated;
            SetTopmostState(popup);
        }

        static void PopupOnClosed(object sender, EventArgs e)
        {
            if (sender is Popup popup && Popups.TryGetValue(popup, out var parentWindow))
            {
                Popups.Remove(popup);
                //如果字典中还存在当前窗口，则不能取消事件订阅
                if (Popups.ContainsValue(parentWindow)) return;
                parentWindow.Activated -= OnParentWindowActivated;
                parentWindow.Deactivated -= OnParentWindowDeactivated;
            }
        }

        private static void OnParentWindowActivated(object sender, EventArgs e)
        {
            if (sender is Window parentWindow)
            {
                var popups = Popups.Keys.TakeWhile(k => Popups.TryGetValue(k, out var window) && window == parentWindow);
                foreach (var popup in popups)
                {
                    SetTopmostState(popup);
                }
            }
        }

        private static void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            //Parent Window Deactivated
            if (sender is Window parentWindow)
            {
                var popups = Popups.Keys.TakeWhile(k => Popups.TryGetValue(k, out var window) && window == parentWindow);
                foreach (var popup in popups)
                {
                    SetTopmostState(popup);
                }
            }
        }

        static void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                if (element.Parent is Popup popup)
                {
                    SetTopmostState(popup);
                    if (Popups.TryGetValue(popup, out var parentWindow))
                    {
                        if (!parentWindow.IsActive)
                        {
                            parentWindow.Activate();
                        }
                    }
                }
            }
        }

        #endregion

        #region 选择性置顶显示

        private static void SetTopmostState(Popup popup)
        {
            var isTopmostPopup = !GetTopmostInCurrentWindow(popup);

            if (popup?.Child == null)
                return;

            if (!(PresentationSource.FromVisual(popup.Child) is HwndSource hwndSource))
                return;
            var hwnd = hwndSource.Handle;

            if (!GetWindowRect(hwnd, out var rect))
                return;

            if (isTopmostPopup)
            {
                SetWindowPos(hwnd, HWND_TOPMOST, rect.Left, rect.Top, (int)popup.Width, (int)popup.Height, TOPMOST_FLAGS);
            }
            else
            {
                // 重新激活Topmost，需要bottom->top->notop
                SetWindowPos(hwnd, HWND_BOTTOM, rect.Left, rect.Top, (int)popup.Width, (int)popup.Height, TOPMOST_FLAGS);
                SetWindowPos(hwnd, HWND_TOP, rect.Left, rect.Top, (int)popup.Width, (int)popup.Height, TOPMOST_FLAGS);
                SetWindowPos(hwnd, HWND_NOTOPMOST, rect.Left, rect.Top, (int)popup.Width, (int)popup.Height, TOPMOST_FLAGS);
            }
        }

        #endregion

        #region 窗口消息

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT

        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOREDRAW = 0x0008;
        const uint SWP_NOACTIVATE = 0x0010;

        const uint SWP_NOOWNERZORDER = 0x0200;
        const uint SWP_NOSENDCHANGING = 0x0400;

        //很重要，窗口切换等需要将popup显示层级重新刷新
        const uint TOPMOST_FLAGS =
            SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOMOVE | SWP_NOREDRAW | SWP_NOSENDCHANGING;


        #endregion

        #region private fileds

        private static readonly Dictionary<Popup, Window> Popups = new Dictionary<Popup, Window>();

        #endregion

        #endregion

        #region 辅助方法

        /// <summary>
        /// 获取目标元素所在屏幕的边界（屏幕坐标）
        /// </summary>
        /// <param name="placementTarget">目标元素</param>
        /// <returns></returns>
        public static Rect GetRelatedScreenBounds(FrameworkElement placementTarget)
        {
            var targetCenterNative = placementTarget.PointToScreen(new Point(placementTarget.ActualWidth / 2, placementTarget.ActualHeight / 2));
            var targetCoordinates = new System.Drawing.Point(Convert.ToInt32(targetCenterNative.X), Convert.ToInt32(targetCenterNative.Y));
            var targetScreen = Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(targetCoordinates))
                               ?? (from screen in Screen.AllScreens
                                   let center = screen.Bounds.Location + new Size(screen.Bounds.Size.Width / 2, screen.Bounds.Size.Height / 2)
                                   let deltaX = center.X - targetCoordinates.X
                                   let deltaY = center.Y - targetCoordinates.Y
                                   orderby deltaX * deltaX + deltaY * deltaY
                                   select screen).First();
            var transform = GetTransformFromDevice(placementTarget);
            var boundsPos = transform.Transform(new Point(targetScreen.Bounds.Left, targetScreen.Bounds.Top));
            var boundsSize = transform.Transform(new Point(targetScreen.Bounds.Right, targetScreen.Bounds.Bottom)) - boundsPos;
            return new Rect(boundsPos, boundsSize);
        }
        private static Matrix GetTransformFromDevice(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source?.CompositionTarget != null) return source.CompositionTarget.TransformFromDevice;
            return Matrix.Identity;
        }

        #endregion
    }
}
