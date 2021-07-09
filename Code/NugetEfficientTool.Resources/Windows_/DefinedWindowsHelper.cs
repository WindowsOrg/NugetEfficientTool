using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace NugetEfficientTool.Resources
{
    public static class DefinedWindowsHelper
    {
        public static readonly DependencyProperty DefinedWindowTypeProperty = DependencyProperty.RegisterAttached(
            "propertyName", typeof(DefinedWindowType), typeof(DefinedWindowsHelper),
            new PropertyMetadata(default(DefinedWindowType), OnDefinedWindowTypePropertyChanged));

        private static void OnDefinedWindowTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && e.NewValue is DefinedWindowType definedWindowType)
            {
                switch (definedWindowType)
                {
                    case DefinedWindowType.GrayTitleWindow:
                    case DefinedWindowType.GrayTitleDialog:
                        {
                            window.AllowsTransparency = true;
                            window.WindowStyle = WindowStyle.None;
                            window.Background = Brushes.Transparent;

                            if (window.IsLoaded)
                            {
                                SetWindowGrayStyle(window, definedWindowType);
                            }
                            else
                            {
                                window.Loaded -= WindowOnLoaded;
                                window.Loaded += WindowOnLoaded;
                                void WindowOnLoaded(object sender, RoutedEventArgs args)
                                {
                                    window.Loaded -= WindowOnLoaded;
                                    SetWindowGrayStyle(window, definedWindowType);
                                }
                            }
                        }
                        break;
                }
            }

        }

        private static void SetWindowGrayStyle(Window window, DefinedWindowType definedWindowType)
        {
            window.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#D0D1D6");
            var rootBorder = new Border()
            {
                BorderBrush = Brushes.Gainsboro,
                BorderThickness = new Thickness(1)
            };

            var mainGrid = new Grid();
            mainGrid.Background = Brushes.White;
            mainGrid.ClipToBounds = true;
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition());
            WindowChrome.SetIsHitTestVisibleInChrome(window, true);
            rootBorder.Child = mainGrid;

            var windowHeaderView = new WindowHeaderView(definedWindowType);
            windowHeaderView.SetValue(Grid.RowProperty, 0);
            mainGrid.Children.Add(windowHeaderView);

            var contentGrid = new Grid();
            contentGrid.SetValue(Grid.RowProperty, 1);
            if (window.Content != null && window.Content is UIElement uiElement)
            {
                window.Content = null;
                contentGrid.Children.Add(uiElement);
            }
            mainGrid.Children.Add(contentGrid);

            window.Content = rootBorder;
        }

        public static void SetDefinedWindowType(DependencyObject element, DefinedWindowType value)
        {
            element.SetValue(DefinedWindowTypeProperty, value);
        }

        public static DefinedWindowType GetDefinedWindowType(DependencyObject element)
        {
            return (DefinedWindowType)element.GetValue(DefinedWindowTypeProperty);
        }
    }

    public enum DefinedWindowType
    {
        None,
        /// <summary>
        /// 灰色背景标题栏
        /// </summary>
        GrayTitleWindow,
        /// <summary>
        /// 灰色背景
        /// </summary>
        GrayTitleDialog,
    }
}
