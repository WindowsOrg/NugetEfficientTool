using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 窗口附加属性
    /// </summary>
    public static class WindowHelper
    {
        #region 窗口抖动
        /// <summary>
        /// 抖动
        /// </summary>
        public static readonly DependencyProperty IsModalWindowProperty = DependencyProperty.RegisterAttached("IsModalWindow",
            typeof(bool), typeof(WindowHelper), new PropertyMetadata(default(bool), OnIsModelWindowChangedCallback));

        private static void OnIsModelWindowChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue) return;
            var window = d as Window;
            if (window == null) return;
            window.Loaded -= Window_Loaded;
            window.Loaded += Window_Loaded;
            window.Unloaded += (s, ee) =>
            {
                SetLoadedStoryboard(window, null);
                SetTwinkleStoryboard(window, null);
                UnregisterWindow(window);
            };
        }
        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Window window = (Window)sender;
            RefreshWindowEffectStoryboard(window);
            GetLoadedStoryboard(window)?.Begin();
            RegisterWindow(window);
        }

        private static void RefreshWindowEffectStoryboard(Window window)
        {
            if (window?.Owner == null)
            {
                return;
            }
            int? scaleTransformIndex = GetTransformIndex(window);

            BuildModalLoadedStoryboard(window, scaleTransformIndex);
            BuildModalTwinkleStoryboard(window, scaleTransformIndex);
            var hwnd = new WindowInteropHelper(window.Owner).Handle;
            if (hwnd != IntPtr.Zero)
            {
                var hwndSource = HwndSource.FromHwnd(hwnd);
                hwndSource?.AddHook(WndProc);
            }
            RegisterWindow(window);
        }
        private static void BuildModalLoadedStoryboard(Window window, int? scaleTransformIndex)
        {
            var scaleXDoubleAnimation = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromMilliseconds(300) };
            var scaleYDoubleAnimation = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromMilliseconds(300) };

            scaleXDoubleAnimation.EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.2 };
            scaleYDoubleAnimation.EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.2 };

            Storyboard.SetTarget(scaleXDoubleAnimation, window);
            Storyboard.SetTarget(scaleYDoubleAnimation, window);
            Storyboard.SetTargetProperty(scaleXDoubleAnimation,
                scaleTransformIndex == null
                    ? new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)")
                    : new PropertyPath(
                        $"(UIElement.RenderTransform).(TransformGroup.Children)[{scaleTransformIndex}].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(scaleYDoubleAnimation,
                scaleTransformIndex == null
                    ? new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)")
                    : new PropertyPath(
                        $"(UIElement.RenderTransform).(TransformGroup.Children)[{scaleTransformIndex}].(ScaleTransform.ScaleY)"));

            SetLoadedStoryboard(window, new Storyboard
            {
                Children = new TimelineCollection { scaleXDoubleAnimation, scaleYDoubleAnimation }
            });
        }

        private static void BuildModalTwinkleStoryboard(Window window, int? scaleTransformIndex)
        {
            var scaleXDoubleAnimation = new DoubleAnimationUsingKeyFrames();
            var scaleYDoubleAnimation = new DoubleAnimationUsingKeyFrames();

            scaleXDoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(0),
                Value = 1.0
            });
            scaleXDoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(100),
                Value = 0.9
            });
            scaleXDoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(200),
                Value = 1.0
            });

            scaleYDoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(0),
                Value = 1.0
            });
            scaleYDoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(100),
                Value = 0.9
            });
            scaleYDoubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame
            {
                KeyTime = TimeSpan.FromMilliseconds(200),
                Value = 1.0
            });

            Storyboard.SetTarget(scaleXDoubleAnimation, window);
            Storyboard.SetTarget(scaleYDoubleAnimation, window);
            Storyboard.SetTargetProperty(scaleXDoubleAnimation,
                scaleTransformIndex == null
                    ? new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)")
                    : new PropertyPath(
                        $"(UIElement.RenderTransform).(TransformGroup.Children)[{scaleTransformIndex}].(ScaleTransform.ScaleX)"));
            Storyboard.SetTargetProperty(scaleYDoubleAnimation,
                scaleTransformIndex == null
                    ? new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)")
                    : new PropertyPath(
                        $"(UIElement.RenderTransform).(TransformGroup.Children)[{scaleTransformIndex}].(ScaleTransform.ScaleY)"));

            SetTwinkleStoryboard(window, new Storyboard
            {
                Children = new TimelineCollection { scaleXDoubleAnimation, scaleYDoubleAnimation }
            });
        }
        private static int? GetTransformIndex(Window window)
        {
            int? scaleTransformIndex = null;
            var transformGroup = window.RenderTransform as TransformGroup;
            if (transformGroup != null)
            {
                for (var i = 0; i < transformGroup.Children.Count; i++)
                {
                    if (!(transformGroup.Children[i] is ScaleTransform)) continue;
                    scaleTransformIndex = i;
                    break;
                }
                if (scaleTransformIndex == null)
                {
                    transformGroup.Children.Add(new ScaleTransform());
                    scaleTransformIndex = transformGroup.Children.Count - 1;
                }
            }
            else
            {
                var scaleTransform = window.RenderTransform as ScaleTransform;
                if (scaleTransform == null) window.RenderTransform = new ScaleTransform();
            }
            window.RenderTransformOrigin = new Point(0.5, 0.5);
            return scaleTransformIndex;
        }
        private static Dictionary<IntPtr, Window> Windows { get; } = new Dictionary<IntPtr, Window>();

        private static void RegisterWindow(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (Windows.ContainsKey(handle))
            {
                Windows[handle] = window;
            }
            else
            {
                Windows.Add(handle, window);
            }
        }

        private static void UnregisterWindow(Window window)
        {
            var handle = new WindowInteropHelper(window).Handle;
            if (Windows.ContainsKey(handle))
            {
                Windows.Remove(handle);
            }

            // 窗口关闭时，句柄将发生变化（句柄变为 0 ），因此需要如下逻辑
            var windows = Windows.Where(x => ReferenceEquals(x.Value, window)).ToArray();
            foreach (var pair in windows)
            {
                Windows.Remove(pair.Key);
            }
        }
        /// <summary>
        /// 监听窗口外部反馈
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != 0x20) return IntPtr.Zero;
            if (lParam.ToInt32() == 0x201fffe)
            {
                if (Windows.Any())
                {
                    // 模态窗口上弹模态窗口，闪烁最后一个窗口即可
                    GetTwinkleStoryboard(Windows.Last().Value)?.Begin();
                }
            }
            return IntPtr.Zero;
        }

        public static void SetIsModalWindow(DependencyObject element, bool value)
        {
            element.SetValue(IsModalWindowProperty, value);
        }

        public static bool GetIsModalWindow(DependencyObject element)
        {
            return (bool)element.GetValue(IsModalWindowProperty);
        }

        private static readonly DependencyProperty LoadedStoryboardProperty = DependencyProperty.RegisterAttached(
            "LoadedStoryboard", typeof(Storyboard), typeof(WindowHelper), new PropertyMetadata(default(Storyboard)));
        private static void SetLoadedStoryboard(Window window, Storyboard value)
        {
            window.SetValue(LoadedStoryboardProperty, value);
        }
        private static Storyboard GetLoadedStoryboard(Window window)
        {
            return (Storyboard)window.GetValue(LoadedStoryboardProperty);
        }

        private static readonly DependencyProperty TwinkleStoryboardProperty = DependencyProperty.RegisterAttached(
            "TwinkleStoryboard", typeof(Storyboard), typeof(WindowHelper), new PropertyMetadata(default(Storyboard)));
        private static void SetTwinkleStoryboard(Window window, Storyboard value)
        {
            window.SetValue(TwinkleStoryboardProperty, value);
        }
        private static Storyboard GetTwinkleStoryboard(Window window)
        {
            return (Storyboard)window.GetValue(TwinkleStoryboardProperty);
        }
        #endregion
    }
}
