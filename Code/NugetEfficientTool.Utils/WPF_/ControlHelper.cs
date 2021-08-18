using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace NugetEfficientTool.Utils
{
    public class ControlHelper
    {
        #region 为控件设定风格色，区别于前景色 AccentBrush

        /// <summary>
        /// 附加属性，用于设定控件的风格色，不占用控件原有的前景色和背景色
        /// </summary>
        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.RegisterAttached("AccentBrush", typeof(Brush), typeof(ControlHelper),
                new PropertyMetadata(null));

        public static Brush GetAccentBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(AccentBrushProperty);
        }

        public static void SetAccentBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(AccentBrushProperty, value);
        }

        #endregion

        #region 控件内部图片源 Image

        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached(
            "Image", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(default(ImageSource)));

        public static void SetImage(DependencyObject element, ImageSource value)
        {
            element.SetValue(ImageProperty, value);
        }

        public static ImageSource GetImage(DependencyObject element)
        {
            return (ImageSource)element.GetValue(ImageProperty);
        }

        public static readonly DependencyProperty ImageHoverProperty = DependencyProperty.RegisterAttached(
            "ImageHover", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(default(ImageSource)));

        public static void SetImageHover(DependencyObject element, ImageSource value)
        {
            element.SetValue(ImageHoverProperty, value);
        }

        public static ImageSource GetImageHover(DependencyObject element)
        {
            return (ImageSource)element.GetValue(ImageHoverProperty);
        }

        public static readonly DependencyProperty ImagePressedProperty = DependencyProperty.RegisterAttached(
            "ImagePressed", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(default(ImageSource)));

        public static void SetImagePressed(DependencyObject element, ImageSource value)
        {
            element.SetValue(ImagePressedProperty, value);
        }

        public static ImageSource GetImagePressed(DependencyObject element)
        {
            return (ImageSource)element.GetValue(ImagePressedProperty);
        }

        #endregion

        #region 指定控件内图片的宽高 ImageSize

        public static readonly DependencyProperty ImageSizeProperty = DependencyProperty.RegisterAttached(
            "ImageSize", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetImageSize(DependencyObject element, double value)
        {
            element.SetValue(ImageSizeProperty, value);
        }

        public static double GetImageSize(DependencyObject element)
        {
            return (double)element.GetValue(ImageSizeProperty);
        }

        #endregion

        #region 为控件附加圆角属性 CornerRadius

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlHelper),
                new PropertyMetadata(default(CornerRadius)));

        #endregion

        #region 为控件附加内容 Content

        /// <summary>
        /// 为没有 Content 属性的控件标识附加属性。
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content", typeof(object), typeof(ControlHelper), new PropertyMetadata(default(object)));

        public static void SetContent(DependencyObject element, object value)
        {
            element.SetValue(ContentProperty, value);
        }

        public static object GetContent(DependencyObject element)
        {
            return element.GetValue(ContentProperty);
        }

        /// <summary>
        /// Content2 属性的控件标识附加属性。
        /// </summary>
        public static readonly DependencyProperty Content2Property = DependencyProperty.RegisterAttached(
            "Content2", typeof(object), typeof(ControlHelper), new PropertyMetadata(default(object)));

        public static void SetContent2(DependencyObject element, object value)
        {
            element.SetValue(Content2Property, value);
        }

        public static object GetContent2(DependencyObject element)
        {
            return element.GetValue(Content2Property);
        }
        #endregion

        #region Geometry

        public static readonly DependencyProperty GeometryProperty = DependencyProperty.RegisterAttached(
            "Geometry", typeof(Geometry), typeof(ControlHelper), new PropertyMetadata(default(Geometry)));

        public static void SetGeometry(DependencyObject element, Geometry value)
        {
            element.SetValue(GeometryProperty, value);
        }

        public static Geometry GetGeometry(DependencyObject element)
        {
            return (Geometry)element.GetValue(GeometryProperty);
        }

        public static readonly DependencyProperty Geometry2Property = DependencyProperty.RegisterAttached(
            "Geometry2", typeof(Geometry), typeof(ControlHelper), new PropertyMetadata(default(Geometry)));

        public static void SetGeometry2(DependencyObject element, Geometry value)
        {
            element.SetValue(Geometry2Property, value);
        }

        public static Geometry GetGeometry2(DependencyObject element)
        {
            return (Geometry)element.GetValue(Geometry2Property);
        }

        #endregion

        #region 为控件提供圆角裁剪

        public static readonly DependencyProperty ClipCornerRadiusProperty = DependencyProperty.RegisterAttached(
            "ClipCornerRadius", typeof(double), typeof(ControlHelper),
            new PropertyMetadata(0.0, ClipCornerRadiusPropertyChangedCallback));

        public static void SetClipCornerRadius(UIElement element, double value)
        {
            element.SetValue(ClipCornerRadiusProperty, value);
        }

        public static double GetClipCornerRadius(UIElement element)
        {
            return (double)element.GetValue(ClipCornerRadiusProperty);
        }

        private static void ClipCornerRadiusPropertyChangedCallback(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement source = (UIElement)d;
            double newValue = (double)e.NewValue;

            RectangleGeometry rectangle = source.Clip as RectangleGeometry;
            if (source.Clip != null && rectangle == null)
            {
                throw new InvalidOperationException(
                    $"{typeof(ControlHelper).FullName}.{ClipCornerRadiusProperty.Name} " +
                    $"属性需要使用到 {source.GetType().FullName}.{UIElement.ClipProperty.Name} " +
                    "属性，请不要在设置此属性的同时设置它。");
            }
            rectangle = rectangle ?? new RectangleGeometry();
            rectangle.RadiusX = newValue;
            rectangle.RadiusY = newValue;
            source.Clip = rectangle;

            MultiBinding multiBinding = BindingOperations.GetMultiBinding(rectangle, RectangleGeometry.RectProperty);
            if (multiBinding == null)
            {
                multiBinding = new MultiBinding
                {
                    Converter = new SizeToClipRectConverter(),
                };
                multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name)
                {
                    Source = source,
                    Mode = BindingMode.OneWay,
                });
                multiBinding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name)
                {
                    Source = source,
                    Mode = BindingMode.OneWay,
                });
                BindingOperations.SetBinding(rectangle, RectangleGeometry.RectProperty, multiBinding);
            }
        }

        #endregion

        #region Hover,Pressed 状态颜色(兼容之前的控件，暂时不删除，待标准化完成时删除)
        public static readonly DependencyProperty TextBrushProperty = DependencyProperty.RegisterAttached(
            "TextBrush", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetTextBrush(DependencyObject element, Brush value)
        {
            element.SetValue(TextBrushProperty, value);
        }

        public static Brush GetTextBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(TextBrushProperty);
        }

        public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.RegisterAttached(
            "HoverBrush", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetHoverBrush(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBrushProperty, value);
        }

        public static Brush GetHoverBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBrushProperty);
        }

        public static readonly DependencyProperty PressedBrushProperty = DependencyProperty.RegisterAttached(
            "PressedBrush", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetPressedBrush(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBrushProperty, value);
        }

        public static Brush GetPressedBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBrushProperty);
        }

        #endregion

        #region Visibility
        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.RegisterAttached(
            "Visibility", typeof(Visibility), typeof(ControlHelper), new PropertyMetadata(Visibility.Visible));

        public static void SetVisibility(DependencyObject element, Visibility value)
        {
            element.SetValue(VisibilityProperty, value);
        }

        public static Visibility GetVisibility(DependencyObject element)
        {
            return (Visibility?)element.GetValue(VisibilityProperty) ?? Visibility.Visible;
        }


        #endregion

        #region 用于描述：控件内部图片在不同状态下是否只使用一张图片

        public static readonly DependencyProperty IsUseSameImageProperty = DependencyProperty.RegisterAttached(
            "IsUseSameImage", typeof(bool), typeof(ControlHelper), new PropertyMetadata(default(bool)));

        public static void SetIsUseSameImage(DependencyObject element, bool value)
        {
            element.SetValue(IsUseSameImageProperty, value);
        }

        public static bool GetIsUseSameImage(DependencyObject element)
        {
            return (bool)element.GetValue(IsUseSameImageProperty);
        }

        #endregion

        #region 用于描述：控件在禁用时的透明度

        public static readonly DependencyProperty DisabledOpacityProperty = DependencyProperty.RegisterAttached(
            "DisabledOpacity", typeof(double), typeof(ControlHelper), new PropertyMetadata(0.4));

        public static void SetDisabledOpacity(DependencyObject element, double value)
        {
            element.SetValue(DisabledOpacityProperty, value);
        }

        public static double GetDisabledOpacity(DependencyObject element)
        {
            return (double)element.GetValue(DisabledOpacityProperty);
        }

        #endregion

        #region 用于描述：控件内部文本前景色

        public static readonly DependencyProperty ForegroundNormalProperty = DependencyProperty.RegisterAttached(
            "ForegroundNormal", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetForegroundNormal(DependencyObject element, Brush value)
        {
            element.SetValue(ForegroundNormalProperty, value);
        }

        public static Brush GetForegroundNormal(DependencyObject element)
        {
            return (Brush)element.GetValue(ForegroundNormalProperty);
        }

        public static readonly DependencyProperty ForegroundHoverProperty = DependencyProperty.RegisterAttached(
            "ForegroundHover", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetForegroundHover(DependencyObject element, Brush value)
        {
            element.SetValue(ForegroundHoverProperty, value);
        }

        public static Brush GetForegroundHover(DependencyObject element)
        {
            return (Brush)element.GetValue(ForegroundHoverProperty);
        }

        public static readonly DependencyProperty ForegroundPressedProperty = DependencyProperty.RegisterAttached(
            "ForegroundPressed", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetForegroundPressed(DependencyObject element, Brush value)
        {
            element.SetValue(ForegroundPressedProperty, value);
        }

        public static Brush GetForegroundPressed(DependencyObject element)
        {
            return (Brush)element.GetValue(ForegroundPressedProperty);
        }

        public static readonly DependencyProperty ForegroundCheckedProperty = DependencyProperty.RegisterAttached(
            "ForegroundChecked", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetForegroundChecked(DependencyObject element, Brush value)
        {
            element.SetValue(ForegroundCheckedProperty, value);
        }

        public static Brush GetForegroundChecked(DependencyObject element)
        {
            return (Brush)element.GetValue(ForegroundCheckedProperty);
        }

        #endregion

        #region 用于描述：控件内部图标填充色

        public static readonly DependencyProperty FillNormalProperty = DependencyProperty.RegisterAttached(
            "FillNormal", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetFillNormal(DependencyObject element, Brush value)
        {
            element.SetValue(FillNormalProperty, value);
        }

        public static Brush GetFillNormal(DependencyObject element)
        {
            return (Brush)element.GetValue(FillNormalProperty);
        }

        public static readonly DependencyProperty FillHoverProperty = DependencyProperty.RegisterAttached(
            "FillHover", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetFillHover(DependencyObject element, Brush value)
        {
            element.SetValue(FillHoverProperty, value);
        }

        public static Brush GetFillHover(DependencyObject element)
        {
            return (Brush)element.GetValue(FillHoverProperty);
        }

        public static readonly DependencyProperty FillPressedProperty = DependencyProperty.RegisterAttached(
            "FillPressed", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetFillPressed(DependencyObject element, Brush value)
        {
            element.SetValue(FillPressedProperty, value);
        }

        public static Brush GetFillPressed(DependencyObject element)
        {
            return (Brush)element.GetValue(FillPressedProperty);
        }

        public static readonly DependencyProperty FillCheckedProperty = DependencyProperty.RegisterAttached(
            "FillChecked", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetFillChecked(DependencyObject element, Brush value)
        {
            element.SetValue(FillCheckedProperty, value);
        }

        public static Brush GetFillChecked(DependencyObject element)
        {
            return (Brush)element.GetValue(FillCheckedProperty);
        }

        public static readonly DependencyProperty FillUncheckedProperty = DependencyProperty.RegisterAttached(
            "FillUnchecked", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(default(Brush)));

        public static void SetFillUnchecked(DependencyObject element, Brush value)
        {
            element.SetValue(FillUncheckedProperty, value);
        }

        public static Brush GetFillUnchecked(DependencyObject element)
        {
            return (Brush)element.GetValue(FillUncheckedProperty);
        }

        #endregion

        #region 用于描述：控件内部背景色

        public static readonly DependencyProperty BackgroundNormalProperty = DependencyProperty.RegisterAttached(
            "BackgroundNormal", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetBackgroundNormal(DependencyObject element, Brush value)
        {
            element.SetValue(BackgroundNormalProperty, value);
        }

        public static Brush GetBackgroundNormal(DependencyObject element)
        {
            return (Brush)element.GetValue(BackgroundNormalProperty);
        }

        public static readonly DependencyProperty BackgroundHoverProperty = DependencyProperty.RegisterAttached(
            "BackgroundHover", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetBackgroundHover(DependencyObject element, Brush value)
        {
            element.SetValue(BackgroundHoverProperty, value);
        }

        public static Brush GetBackgroundHover(DependencyObject element)
        {
            return (Brush)element.GetValue(BackgroundHoverProperty);
        }

        public static readonly DependencyProperty BackgroundPressedProperty = DependencyProperty.RegisterAttached(
            "BackgroundPressed", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetBackgroundPressed(DependencyObject element, Brush value)
        {
            element.SetValue(BackgroundPressedProperty, value);
        }

        public static Brush GetBackgroundPressed(DependencyObject element)
        {
            return (Brush)element.GetValue(BackgroundPressedProperty);
        }

        public static readonly DependencyProperty BackgroundCheckedProperty = DependencyProperty.RegisterAttached(
            "BackgroundChecked", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetBackgroundChecked(DependencyObject element, Brush value)
        {
            element.SetValue(BackgroundCheckedProperty, value);
        }

        public static Brush GetBackgroundChecked(DependencyObject element)
        {
            return (Brush)element.GetValue(BackgroundCheckedProperty);
        }

        #endregion

        #region 用于描述：控件边框颜色

        public static readonly DependencyProperty BorderBrushNormalProperty = DependencyProperty.RegisterAttached(
            "BorderBrushNormal", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetBorderBrushNormal(DependencyObject element, Brush value)
        {
            element.SetValue(BorderBrushNormalProperty, value);
        }

        public static Brush GetBorderBrushNormal(DependencyObject element)
        {
            return (Brush)element.GetValue(BorderBrushNormalProperty);
        }

        public static readonly DependencyProperty BorderBrushHoverProperty = DependencyProperty.RegisterAttached(
            "BorderBrushHover", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetBorderBrushHover(DependencyObject element, Brush value)
        {
            element.SetValue(BorderBrushHoverProperty, value);
        }

        public static Brush GetBorderBrushHover(DependencyObject element)
        {
            return (Brush)element.GetValue(BorderBrushHoverProperty);
        }

        public static readonly DependencyProperty BorderBrushPressedProperty = DependencyProperty.RegisterAttached(
            "BorderBrushPressed", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetBorderBrushPressed(DependencyObject element, Brush value)
        {
            element.SetValue(BorderBrushPressedProperty, value);
        }

        public static Brush GetBorderBrushPressed(DependencyObject element)
        {
            return (Brush)element.GetValue(BorderBrushPressedProperty);
        }

        public static readonly DependencyProperty BorderBrushCheckedProperty = DependencyProperty.RegisterAttached(
            "BorderBrushChecked", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Black));

        public static void SetBorderBrushChecked(DependencyObject element, Brush value)
        {
            element.SetValue(BorderBrushCheckedProperty, value);
        }

        public static Brush GetBorderBrushChecked(DependencyObject element)
        {
            return (Brush)element.GetValue(BorderBrushCheckedProperty);
        }

        #endregion

        #region 用于描述：控件内部Image或Geometry的边距

        public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.RegisterAttached(
            "ImageMargin", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetImageMargin(DependencyObject element, Thickness value)
        {
            element.SetValue(ImageMarginProperty, value);
        }

        public static Thickness GetImageMargin(DependencyObject element)
        {
            return (Thickness)element.GetValue(ImageMarginProperty);
        }

        #endregion

        #region 用于描述：控件内部Text的边距

        public static readonly DependencyProperty TextMarginProperty = DependencyProperty.RegisterAttached(
            "TextMargin", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetTextMargin(DependencyObject element, Thickness value)
        {
            element.SetValue(TextMarginProperty, value);
        }

        public static Thickness GetTextMargin(DependencyObject element)
        {
            return (Thickness)element.GetValue(TextMarginProperty);
        }

        #endregion

        #region 用于描述：是否当鼠标在文本上时，显示手势鼠标样式

        public static readonly DependencyProperty IsShowHandWhenMouseOverTextProperty = DependencyProperty.RegisterAttached(
            "IsShowHandWhenMouseOverText", typeof(bool), typeof(ControlHelper), new PropertyMetadata(default(bool)));

        public static void SetIsShowHandWhenMouseOverText(DependencyObject element, bool value)
        {
            element.SetValue(IsShowHandWhenMouseOverTextProperty, value);
        }

        public static bool GetIsShowHandWhenMouseOverText(DependencyObject element)
        {
            return (bool)element.GetValue(IsShowHandWhenMouseOverTextProperty);
        }

        #endregion

        #region 用于描述：文字下样式

        public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.RegisterAttached(
            "TextDecorations", typeof(TextDecorationCollection), typeof(ControlHelper), new PropertyMetadata(default(TextDecorationCollection)));

        public static void SetTextDecorations(DependencyObject element, TextDecorationCollection value)
        {
            element.SetValue(TextDecorationsProperty, value);
        }

        public static TextDecorationCollection GetTextDecorations(DependencyObject element)
        {
            return (TextDecorationCollection)element.GetValue(TextDecorationsProperty);
        }

        #endregion

        #region 用于描述：文本裁剪行为

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.RegisterAttached(
            "TextTrimming", typeof(TextTrimming), typeof(ControlHelper), new PropertyMetadata(TextTrimming.None));

        public static void SetTextTrimming(DependencyObject element, TextTrimming value)
        {
            element.SetValue(TextTrimmingProperty, value);
        }

        public static TextTrimming GetTextTrimming(DependencyObject element)
        {
            return (TextTrimming)element.GetValue(TextTrimmingProperty);
        }

        #endregion

        #region 用于描述：控件中图片源填充方式

        public static readonly DependencyProperty ImageStretchProperty = DependencyProperty.RegisterAttached(
            "ImageStretch", typeof(Stretch), typeof(ControlHelper), new PropertyMetadata(default(Stretch)));

        public static void SetImageStretch(DependencyObject element, Stretch value)
        {
            element.SetValue(ImageStretchProperty, value);
        }

        public static Stretch GetImageStretch(DependencyObject element)
        {
            return (Stretch)element.GetValue(ImageStretchProperty);
        }

        #endregion

        #region 用于描述：带有透明度蒙层的填充色

        public static readonly DependencyProperty CoverFillNormalProperty = DependencyProperty.RegisterAttached(
            "CoverFillNormal", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetCoverFillNormal(DependencyObject element, Brush value)
        {
            element.SetValue(CoverFillNormalProperty, value);
        }

        public static Brush GetCoverFillNormal(DependencyObject element)
        {
            return (Brush)element.GetValue(CoverFillNormalProperty);
        }

        public static readonly DependencyProperty CoverFillHoverProperty = DependencyProperty.RegisterAttached(
            "CoverFillHover", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetCoverFillHover(DependencyObject element, Brush value)
        {
            element.SetValue(CoverFillHoverProperty, value);
        }

        public static Brush GetCoverFillHover(DependencyObject element)
        {
            return (Brush)element.GetValue(CoverFillHoverProperty);
        }

        public static readonly DependencyProperty CoverFillPressedProperty = DependencyProperty.RegisterAttached(
            "CoverFillPressed", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetCoverFillPressed(DependencyObject element, Brush value)
        {
            element.SetValue(CoverFillPressedProperty, value);
        }

        public static Brush GetCoverFillPressed(DependencyObject element)
        {
            return (Brush)element.GetValue(CoverFillPressedProperty);
        }

        public static readonly DependencyProperty CoverFillCheckedProperty = DependencyProperty.RegisterAttached(
            "CoverFillChecked", typeof(Brush), typeof(ControlHelper), new PropertyMetadata(Brushes.Transparent));

        public static void SetCoverFillChecked(DependencyObject element, Brush value)
        {
            element.SetValue(CoverFillCheckedProperty, value);
        }

        public static Brush GetCoverFillChecked(DependencyObject element)
        {
            return (Brush)element.GetValue(CoverFillCheckedProperty);
        }

        #endregion

        #region 用于描述：控件阴影半径

        public static readonly DependencyProperty EffectBlurRadiusProperty = DependencyProperty.RegisterAttached(
            "EffectBlurRadius", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetEffectBlurRadius(DependencyObject element, double value)
        {
            element.SetValue(EffectBlurRadiusProperty, value);
        }

        public static double GetEffectBlurRadius(DependencyObject element)
        {
            return (double)element.GetValue(EffectBlurRadiusProperty);
        }

        #endregion

        #region 用于描述：控件阴影投影距离

        public static readonly DependencyProperty EffectShadowDepthProperty = DependencyProperty.RegisterAttached(
            "EffectShadowDepth", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetEffectShadowDepth(DependencyObject element, double value)
        {
            element.SetValue(EffectShadowDepthProperty, value);
        }

        public static double GetEffectShadowDepth(DependencyObject element)
        {
            return (double)element.GetValue(EffectShadowDepthProperty);
        }

        #endregion

        #region 用于描述：控件投影颜色

        public static readonly DependencyProperty EffectColorProperty = DependencyProperty.RegisterAttached(
            "EffectColor", typeof(Color), typeof(ControlHelper), new PropertyMetadata(Colors.Transparent));

        public static void SetEffectColor(DependencyObject element, Color value)
        {
            element.SetValue(EffectColorProperty, value);
        }

        public static Color GetEffectColor(DependencyObject element)
        {
            return (Color)element.GetValue(EffectColorProperty);
        }

        #endregion

        #region 用于描述：控件投影透明度

        public static readonly DependencyProperty EffectOpacityProperty = DependencyProperty.RegisterAttached(
            "EffectOpacity", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetEffectOpacity(DependencyObject element, double value)
        {
            element.SetValue(EffectOpacityProperty, value);
        }

        public static double GetEffectOpacity(DependencyObject element)
        {
            return (double)element.GetValue(EffectOpacityProperty);
        }

        #endregion

        #region 用于描述：形状边框宽度

        public static readonly DependencyProperty ShapeStrokeThicknessProperty = DependencyProperty.RegisterAttached(
            "ShapeStrokeThickness", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetShapeStrokeThickness(DependencyObject element, double value)
        {
            element.SetValue(ShapeStrokeThicknessProperty, value);
        }

        public static double GetShapeStrokeThickness(DependencyObject element)
        {
            return (double)element.GetValue(ShapeStrokeThicknessProperty);
        }

        #endregion

        #region 用于描述：图片和文字的排列形式是水平还是垂直

        public static readonly DependencyProperty ContentOrientationProperty = DependencyProperty.RegisterAttached(
            "ContentOrientation", typeof(Orientation), typeof(ControlHelper), new PropertyMetadata(Orientation.Horizontal));

        public static void SetContentOrientation(DependencyObject element, Orientation value)
        {
            element.SetValue(ContentOrientationProperty, value);
        }

        public static Orientation GetContentOrientation(DependencyObject element)
        {
            return (Orientation)element.GetValue(ContentOrientationProperty);
        }

        #endregion

        #region 用于描述Image的宽高

        public static readonly DependencyProperty ImageHeightProperty = DependencyProperty.RegisterAttached(
            "ImageHeight", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetImageHeight(DependencyObject element, double value)
        {
            element.SetValue(ImageHeightProperty, value);
        }

        public static double GetImageHeight(DependencyObject element)
        {
            return (double)element.GetValue(ImageHeightProperty);
        }

        public static readonly DependencyProperty ImageWidthProperty = DependencyProperty.RegisterAttached(
            "ImageWidth", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetImageWidth(DependencyObject element, double value)
        {
            element.SetValue(ImageWidthProperty, value);
        }

        public static double GetImageWidth(DependencyObject element)
        {
            return (double)element.GetValue(ImageWidthProperty);
        }
        #endregion

        #region 用于描述：Checked状态时的标记图标源

        public static readonly DependencyProperty SignImageProperty = DependencyProperty.RegisterAttached(
            "SignImage", typeof(ImageSource), typeof(ControlHelper), new PropertyMetadata(null));

        public static void SetSignImage(DependencyObject element, ImageSource value)
        {
            element.SetValue(SignImageProperty, value);
        }

        public static ImageSource GetSignImage(DependencyObject element)
        {
            return (ImageSource)element.GetValue(SignImageProperty);
        }

        #endregion

        #region 用于描述：Checked状态时的标记图标源的水平方向

        public static readonly DependencyProperty SignImageHorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
            "SignImageHorizontalAlignment", typeof(HorizontalAlignment), typeof(ControlHelper), new PropertyMetadata(default(HorizontalAlignment)));

        public static void SetSignImageHorizontalAlignment(DependencyObject element, HorizontalAlignment value)
        {
            element.SetValue(SignImageHorizontalAlignmentProperty, value);
        }

        public static HorizontalAlignment GetSignImageHorizontalAlignment(DependencyObject element)
        {
            return (HorizontalAlignment)element.GetValue(SignImageHorizontalAlignmentProperty);
        }

        #endregion

        #region 用于描述：Checked状态时的标记图标源垂直方向

        public static readonly DependencyProperty SignImageVerticalAlignmentProperty = DependencyProperty.RegisterAttached(
            "SignImageVerticalAlignment", typeof(VerticalAlignment), typeof(ControlHelper), new PropertyMetadata(default(VerticalAlignment)));

        public static void SetSignImageVerticalAlignment(DependencyObject element, VerticalAlignment value)
        {
            element.SetValue(SignImageVerticalAlignmentProperty, value);
        }

        public static VerticalAlignment GetSignImageVerticalAlignment(DependencyObject element)
        {
            return (VerticalAlignment)element.GetValue(SignImageVerticalAlignmentProperty);
        }

        #endregion

        #region 用于描述：Checked状态时的标记图标源的边距

        public static readonly DependencyProperty SignImageMarginProperty = DependencyProperty.RegisterAttached(
            "SignImageMargin", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetSignImageMargin(DependencyObject element, Thickness value)
        {
            element.SetValue(SignImageMarginProperty, value);
        }

        public static Thickness GetSignImageMargin(DependencyObject element)
        {
            return (Thickness)element.GetValue(SignImageMarginProperty);
        }

        #endregion

        #region 用于描述Checked状态的标记图标大小

        public static readonly DependencyProperty SignImageSizeProperty = DependencyProperty.RegisterAttached(
            "SignImageSize", typeof(double), typeof(ControlHelper), new PropertyMetadata(default(double)));

        public static void SetSignImageSize(DependencyObject element, double value)
        {
            element.SetValue(SignImageSizeProperty, value);
        }

        public static double GetSignImageSize(DependencyObject element)
        {
            return (double)element.GetValue(SignImageSizeProperty);
        }

        #endregion

        #region 用于描述在非Checked状态下时边框画刷

        public static readonly DependencyProperty HasUncheckedHintProperty = DependencyProperty.RegisterAttached(
            "HasUncheckedHint", typeof(bool), typeof(ControlHelper), new PropertyMetadata(default(bool)));

        public static void SetHasUncheckedHint(DependencyObject element, bool value)
        {
            element.SetValue(HasUncheckedHintProperty, value);
        }

        public static bool GetHasUncheckedHint(DependencyObject element)
        {
            return (bool)element.GetValue(HasUncheckedHintProperty);
        }

        #endregion

        #region 用于描述：控件是否显示为Warning状态

        public static readonly DependencyProperty WarningStateProperty = DependencyProperty.RegisterAttached(
            "WarningState", typeof(WarningState), typeof(ControlHelper), new PropertyMetadata(WarningState.Normal));

        public static void SetWarningState(DependencyObject element, WarningState value)
        {
            element.SetValue(WarningStateProperty, value);
        }

        public static WarningState GetWarningState(DependencyObject element)
        {
            return (WarningState)element.GetValue(WarningStateProperty);
        }

        #endregion

        #region 水印内容

        public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.RegisterAttached(
            "WatermarkText", typeof(string), typeof(ControlHelper), new PropertyMetadata(default(string)));

        public static void SetWatermarkText(DependencyObject element, string value)
        {
            element.SetValue(WatermarkTextProperty, value);
        }

        public static string GetWatermarkText(DependencyObject element)
        {
            return (string)element.GetValue(WatermarkTextProperty);
        }

        #endregion

        #region 控件内内容边距

        public static readonly DependencyProperty ContentPadingProperty = DependencyProperty.RegisterAttached(
            "ContentPading", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetContentPading(DependencyObject element, Thickness value)
        {
            element.SetValue(ContentPadingProperty, value);
        }

        public static Thickness GetContentPading(DependencyObject element)
        {
            return (Thickness) element.GetValue(ContentPadingProperty);
        }

        #endregion

        #region 控件内部padding

        public static readonly DependencyProperty MarinNormalProperty = DependencyProperty.RegisterAttached(
            "MarinNormal", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetMarinNormal(DependencyObject element, Thickness value)
        {
            element.SetValue(MarinNormalProperty, value);
        }

        public static Thickness GetMarinNormal(DependencyObject element)
        {
            return (Thickness) element.GetValue(MarinNormalProperty);
        }

        public static readonly DependencyProperty MarginHoverProperty = DependencyProperty.RegisterAttached(
            "MarginHover", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetMarginHover(DependencyObject element, Thickness value)
        {
            element.SetValue(MarginHoverProperty, value);
        }

        public static Thickness GetMarginHover(DependencyObject element)
        {
            return (Thickness) element.GetValue(MarginHoverProperty);
        }

        public static readonly DependencyProperty MarginPressedProperty = DependencyProperty.RegisterAttached(
            "MarginPressed", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetMarginPressed(DependencyObject element, Thickness value)
        {
            element.SetValue(MarginPressedProperty, value);
        }

        public static Thickness GetMarginPressed(DependencyObject element)
        {
            return (Thickness) element.GetValue(MarginPressedProperty);
        }

        public static readonly DependencyProperty MarginCheckedProperty = DependencyProperty.RegisterAttached(
            "MarginChecked", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetMarginChecked(DependencyObject element, Thickness value)
        {
            element.SetValue(MarginCheckedProperty, value);
        }

        public static Thickness GetMarginChecked(DependencyObject element)
        {
            return (Thickness) element.GetValue(MarginCheckedProperty);
        }

        #endregion

        #region 控件内部padding

        public static readonly DependencyProperty BorderThicknessNormalProperty = DependencyProperty.RegisterAttached(
            "BorderThicknessNormal", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetBorderThicknessNormal(DependencyObject element, Thickness value)
        {
            element.SetValue(BorderThicknessNormalProperty, value);
        }

        public static Thickness GetBorderThicknessNormal(DependencyObject element)
        {
            return (Thickness)element.GetValue(BorderThicknessNormalProperty);
        }

        public static readonly DependencyProperty BorderThicknessHoverProperty = DependencyProperty.RegisterAttached(
            "BorderThicknessHover", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetBorderThicknessHover(DependencyObject element, Thickness value)
        {
            element.SetValue(BorderThicknessHoverProperty, value);
        }

        public static Thickness GetBorderThicknessHover(DependencyObject element)
        {
            return (Thickness)element.GetValue(BorderThicknessHoverProperty);
        }

        public static readonly DependencyProperty BorderThicknessPressedProperty = DependencyProperty.RegisterAttached(
            "BorderThicknessPressed", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetBorderThicknessPressed(DependencyObject element, Thickness value)
        {
            element.SetValue(BorderThicknessPressedProperty, value);
        }

        public static Thickness GetBorderThicknessPressed(DependencyObject element)
        {
            return (Thickness)element.GetValue(BorderThicknessPressedProperty);
        }

        public static readonly DependencyProperty BorderThicknessCheckedProperty = DependencyProperty.RegisterAttached(
            "BorderThicknessChecked", typeof(Thickness), typeof(ControlHelper), new PropertyMetadata(default(Thickness)));

        public static void SetBorderThicknessChecked(DependencyObject element, Thickness value)
        {
            element.SetValue(BorderThicknessCheckedProperty, value);
        }

        public static Thickness GetBorderThicknessChecked(DependencyObject element)
        {
            return (Thickness)element.GetValue(BorderThicknessCheckedProperty);
        }

        #endregion

        #region HorizontalContentAlignment

        public static readonly DependencyProperty HorizontalContentAlignmentProperty = DependencyProperty.RegisterAttached(
            "HorizontalContentAlignment", typeof(HorizontalAlignment), typeof(ControlHelper), new PropertyMetadata(default(HorizontalAlignment)));

        public static void SetHorizontalContentAlignment(DependencyObject element, HorizontalAlignment value)
        {
            element.SetValue(HorizontalContentAlignmentProperty, value);
        }

        public static HorizontalAlignment GetHorizontalContentAlignment(DependencyObject element)
        {
            return (HorizontalAlignment) element.GetValue(HorizontalContentAlignmentProperty);
        }

        #endregion

        #region MyRegion

        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached(
            "IsFocused", typeof(bool), typeof(ControlHelper), new PropertyMetadata(default(bool)));

        public static void SetIsFocused(DependencyObject element, bool value)
        {
            element.SetValue(IsFocusedProperty, value);
        }

        public static bool GetIsFocused(DependencyObject element)
        {
            return (bool) element.GetValue(IsFocusedProperty);
        }

        #endregion
    }

    public enum WarningState
    {
        Normal,
        Error
    }
}
