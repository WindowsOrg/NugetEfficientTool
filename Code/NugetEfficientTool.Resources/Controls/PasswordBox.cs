using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NugetEfficientTool.Resources
{
    public class PasswordBox : ComboBox
    {
        private System.Windows.Controls.PasswordBox _passwordBox;
        private static string PartPasswordBox = "PART_PasswordBox";

        static PasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordBox), new FrameworkPropertyMetadata(typeof(PasswordBox)));
        }

        public event RoutedEventHandler PasswordChanged;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _passwordBox = GetTemplateChild(PartPasswordBox) as System.Windows.Controls.PasswordBox;
            if (_passwordBox == null) return;

            KeyDown += (s, e) => CapsVisibility = Console.CapsLock ? Visibility.Visible : Visibility.Collapsed;
            _passwordBox.PasswordChanged += (s, e) =>
            {
                PasswordChanged?.Invoke(s,e);
                Password = _passwordBox.Password;
            };
            _passwordBox.GotFocus += (s, e) => { IsPasswordBoxFocused = true; WarningText = ""; };
            _passwordBox.LostFocus += (s, e) =>
            {
                IsPasswordBoxFocused = false;
                CapsVisibility = Visibility.Collapsed;
            };
            GotFocus += (s, e) =>
            {
                _passwordBox.Focus();
                CapsVisibility = Console.CapsLock ? Visibility.Visible : Visibility.Collapsed;
            };
            _passwordBox.PreviewMouseLeftButtonDown += _passwordBox_PreviewMouseLeftButtonDown;
        }

        private void _passwordBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                OnPasswordBoxStylusDown(new StylusDownEventArgs(e.StylusDevice, e.Timestamp));
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == PasswordProperty)
            {
                if (!string.IsNullOrEmpty(_passwordBox?.Password) && string.IsNullOrEmpty((string)e.NewValue))
                    _passwordBox.Password = "";
            }
        }
        /// <summary>
        /// 设置文本为空时的提示信息
        /// </summary>
        public static readonly DependencyProperty TipTextProperty = DependencyProperty.Register("TipText", typeof(string), typeof(PasswordBox));
        public string TipText
        {
            get { return (string)GetValue(TipTextProperty); }
            set { SetValue(TipTextProperty, value); }
        }

        /// <summary>
        /// 设置文本的错误提示信息
        /// </summary>
        public static readonly DependencyProperty WarningTextProperty = DependencyProperty.Register("WarningText", typeof(string), typeof(PasswordBox), new PropertyMetadata(default(string)));
        public string WarningText
        {
            get { return (string)GetValue(WarningTextProperty); }
            set { SetValue(WarningTextProperty, value); }
        }

        /// <summary>
        /// 设置Ok提示图片的显示与隐藏
        /// </summary>
        public static readonly DependencyProperty TipOkVisibilityProperty = DependencyProperty.Register("TipOkVisibility", typeof(Visibility), typeof(PasswordBox), new PropertyMetadata(default(Visibility)));
        public Visibility TipOkVisibility
        {
            get { return (Visibility)GetValue(TipOkVisibilityProperty); }
            set { SetValue(TipOkVisibilityProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordBox), new PropertyMetadata(default(string)));

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty IsPasswordBoxFocusedProperty = DependencyProperty.Register("IsPasswordBoxFocused", typeof(bool), typeof(PasswordBox), new PropertyMetadata(default(bool)));
        public bool IsPasswordBoxFocused
        {
            get { return (bool)GetValue(IsPasswordBoxFocusedProperty); }
            set { SetValue(IsPasswordBoxFocusedProperty, value); }
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(PasswordBox), new PropertyMetadata(default(int)));
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public static readonly DependencyProperty CapsVisibilityProperty = DependencyProperty.Register("CapsVisibility", typeof(Visibility), typeof(PasswordBox), new PropertyMetadata(default(Visibility)));
        public Visibility CapsVisibility
        {
            get { return (Visibility)GetValue(CapsVisibilityProperty); }
            set { SetValue(CapsVisibilityProperty, value); }
        }

        public static readonly DependencyProperty WarningVisibilityProperty = DependencyProperty.Register("WarningVisibility", typeof(Visibility), typeof(PasswordBox), new PropertyMetadata(Visibility.Collapsed));
        public Visibility WarningVisibility
        {
            get { return (Visibility)GetValue(WarningVisibilityProperty); }
            set { SetValue(WarningVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TitleImageSourceProperty = DependencyProperty.Register("TitleImageSource", typeof(ImageSource), typeof(PasswordBox), new PropertyMetadata(default(ImageSource)));
        public ImageSource TitleImageSource
        {
            get { return (ImageSource)GetValue(TitleImageSourceProperty); }
            set { SetValue(TitleImageSourceProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PasswordBox), new PropertyMetadata(new CornerRadius(3)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        public event EventHandler<StylusDownEventArgs> PasswordBoxStylusDown;

        protected virtual void OnPasswordBoxStylusDown(StylusDownEventArgs e)
        {
            PasswordBoxStylusDown?.Invoke(this, e);
        }
    }

    /// <summary>
    /// 为PasswordBox控件的Password增加绑定功能
    /// </summary>
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, Attach));
        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxHelper));

        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }
        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }
        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }
        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }
        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }
        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!GetIsUpdating(passwordBox)) { passwordBox.Password = (string)e.NewValue; }
            passwordBox.PasswordChanged += PasswordChanged;
        }
        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;
            if ((bool)e.OldValue) { passwordBox.PasswordChanged -= PasswordChanged; }
            if ((bool)e.NewValue) { passwordBox.PasswordChanged += PasswordChanged; }
        }
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as System.Windows.Controls.PasswordBox;
            if (passwordBox == null) return;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
    public class KeyboardListener
    {
        public static void HookKeyboard()
        {
            if (_proc != null) return;
            _proc = HookCallback;
            //安装当前线程的钩子
            _threadHookId = InterceptKeys.SetHook(_proc, InterceptKeys.WH_KEYBOARD, InterceptKeys.GetCurrentThreadId());
            //安装全局的钩子
            _globalHookId = InterceptKeys.SetHook(_proc, InterceptKeys.WH_KEYBOARD_LL, 0);
        }

        public static void UnhookKeyboard()
        {
            if (_threadHookId != IntPtr.Zero)
            {
                InterceptKeys.UnhookWindowsHookEx(_threadHookId);
            }

            if (_globalHookId != IntPtr.Zero)
            {
                InterceptKeys.UnhookWindowsHookEx(_globalHookId);
            }

            _proc = null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            return IntPtr.Zero;
            //return InterceptKeys.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// 定义一个局部变量保存，防止被垃圾回收
        /// </summary>
        private static InterceptKeys.LowLevelKeyboardProc _proc;

        private static IntPtr _threadHookId = IntPtr.Zero;
        private static IntPtr _globalHookId = IntPtr.Zero;
    }

    internal static class InterceptKeys
    {
        public delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        public static int WH_KEYBOARD_LL = 13;//全局钩子定义
        public static int WH_KEYBOARD = 2;//私有钩子定义

        public static IntPtr SetHook(LowLevelKeyboardProc proc, int hookType, uint threadId)
        {
            using (var curProcess = Process.GetCurrentProcess())
            {
                using (var curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(hookType, proc, GetModuleHandle(curModule?.ModuleName), threadId);
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint GetCurrentThreadId();
    }
    public static class InputHelper
    {
        public static readonly DependencyProperty PreventKeyBoardHookProperty = DependencyProperty.RegisterAttached(
            "PreventKeyBoardHook", typeof(bool), typeof(InputHelper), new PropertyMetadata(default(bool), PropertyChangedCallback));

        public static void SetPreventKeyBoardHook(DependencyObject element, bool value)
        {
            element.SetValue(PreventKeyBoardHookProperty, value);
        }

        public static bool GetPreventKeyBoardHook(DependencyObject element)
        {
            return (bool)element.GetValue(PreventKeyBoardHookProperty);
        }
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.GotFocus -= ElementOnGotFocus;
                element.LostFocus -= ElementOnLostFocus;
                if ((bool)e.NewValue)
                {
                    element.GotFocus += ElementOnGotFocus;
                    element.LostFocus += ElementOnLostFocus;
                }
            }
        }

        private static void ElementOnLostFocus(object sender, RoutedEventArgs e)
        {
            KeyboardListener.UnhookKeyboard();
        }

        private static void ElementOnGotFocus(object sender, RoutedEventArgs e)
        {
            KeyboardListener.HookKeyboard();
        }
    }
}
