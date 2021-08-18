using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace NugetEfficientTool.Utils
{
            /// <summary>
    /// 此类提供 Popup 自定义定位回调方法
    /// </summary>
    public static class PopupPlacement
    {
        private static readonly DependencyProperty AdjustTransformProperty = DependencyProperty.RegisterAttached(
            "AdjustTransform", typeof(TranslateTransform), typeof(PopupPlacement), new PropertyMetadata(null));

        private static void SetAdjustTransform(Popup element, TranslateTransform value)
        {
            element.SetValue(AdjustTransformProperty, value);
        }

        private static TranslateTransform GetAdjustTransform(Popup element)
        {
            return (TranslateTransform)element.GetValue(AdjustTransformProperty);
        }

        /// <summary>
        /// 在目标元素下方并向左对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback BottomLeft(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(0, targetSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }


        /// <summary>
        ///  在目标元素下方并向左对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="offsetX">相对于目标参考点的水平偏移量</param>
        /// <param name="offsetY">相对于目标参考点的垂直偏移量</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback BottomLeft(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.BottomLeft(outOfScreenEnabled);
        }

        /// <summary>
        ///  在目标元素下方并水平居中对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback BottomCenter(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) / 2, targetSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素下方并水平居中对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="offsetX">相对于目标参考点的水平偏移量</param>
        /// <param name="offsetY">相对于目标参考点的垂直偏移量</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback BottomCenter(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.BottomCenter(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素下方并向右对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback BottomRight(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(targetSize.Width - popupSize.Width, targetSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素下方并向右对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="offsetX">相对于目标参考点的水平偏移量</param>
        /// <param name="offsetY">相对于目标参考点的垂直偏移量</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback BottomRight(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.BottomRight(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素上方并向左对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="outOfScreenEnabled">Popup是否超出屏幕</param>
        /// <returns></returns>
        public static CustomPopupPlacementCallback TopLeft(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback leftBottom = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(0, -popupSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return leftBottom;
        }

        /// <summary>
        /// 在目标元素上方并向左对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="offsetX">相对于目标参考点的水平偏移量</param>
        /// <param name="offsetY">相对于目标参考点的垂直偏移量</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback TopLeft(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.TopLeft(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素上方并水平居中对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback TopCenter(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback centerTop = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) / 2, -popupSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };

            return centerTop;
        }

        /// <summary>
        /// 在目标元素上方并水平居中对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="offsetX">相对于目标参考点的水平偏移量</param>
        /// <param name="offsetY">相对于目标参考点的垂直偏移量</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback TopCenter(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.TopCenter(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素上方并向右对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback TopRight(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(targetSize.Width - popupSize.Width, -popupSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素上方并向右对齐目标元素
        /// </summary>
        /// <param name="popup">弹出框</param>
        /// <param name="offsetX">相对于目标参考点的水平偏移量</param>
        /// <param name="offsetY">相对于目标参考点的垂直偏移量</param>
        /// <param name="outOfScreenEnabled">是否允许超出屏幕</param>
        public static CustomPopupPlacementCallback TopRight(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.TopRight(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素左侧并向下对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback LeftBottom(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(-popupSize.Width, -popupSize.Height + targetSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素左侧并向下对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback LeftBottom(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.LeftBottom(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素左侧并向上对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback LeftTop(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(-popupSize.Width, 0), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素左侧并向上对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback LeftTop(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.LeftTop(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素左侧并垂直居中对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback LeftCenter(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) / 2), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素左侧并向上对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback LeftCenter(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.LeftCenter(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素右侧并向上对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback RightTop(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(targetSize.Width, 0), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素右侧并向上对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback RightTop(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.RightTop(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素右侧并垂直居中对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback RightCenter(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(targetSize.Width, (targetSize.Height - popupSize.Height) / 2), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素右侧并向上对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback RightCenter(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.RightCenter(outOfScreenEnabled);
        }

        /// <summary>
        /// 在目标元素右侧并向下对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback RightBottom(this Popup popup, bool outOfScreenEnabled = false)
        {
            CustomPopupPlacementCallback callback = (popupSize, targetSize, offset) =>
            {
                var placement1 = new CustomPopupPlacement(new Point(targetSize.Width, -popupSize.Height + targetSize.Height), PopupPrimaryAxis.Vertical);
                if (outOfScreenEnabled)
                {
                    AdjustPopup(popup, placement1.Point, popupSize);
                }
                var ttplaces = new[] { placement1 };
                return ttplaces;
            };
            return callback;
        }

        /// <summary>
        /// 在目标元素右侧并向下对齐目标元素
        /// </summary>
        public static CustomPopupPlacementCallback RightBottom(this Popup popup, double offsetX, double offsetY, bool outOfScreenEnabled = false)
        {
            popup.HorizontalOffset = offsetX;
            popup.VerticalOffset = offsetY;
            return popup.RightBottom(outOfScreenEnabled);
        }

        private static void AdjustPopup(Popup popup, Point placementPoint, Size popupSize)
        {
            if (popup.Child == null) return;

            //获取屏幕边界
            var target = (FrameworkElement)popup.PlacementTarget;
            var screenBounds = PopupHelper.GetRelatedScreenBounds(target);

            //获取Popup边界
            var transform = GetTransformFromDevice(target);
            var transformedPlacementPoint = transform.Transform(placementPoint);
            var offsetPoint = new Point(transformedPlacementPoint.X + popup.HorizontalOffset, transformedPlacementPoint.Y + popup.VerticalOffset);
            var nativeTargetPos = target.PointToScreen(new Point());
            var targetPos = transform.Transform(nativeTargetPos);
            var boundsPos = new Point(targetPos.X + offsetPoint.X, targetPos.Y + offsetPoint.Y);
            var popupOrginSize = transform.Transform(new Vector(popupSize.Width, popupSize.Height));
            var bounds = new Rect(boundsPos, popupOrginSize);

            //设置用于调整Popup的平移变换
            var margin = ((FrameworkElement)popup.Child).Margin;
            var oldTransform = popup.Child.RenderTransform;
            var translateTransform = GetAdjustTransform(popup);
            if (translateTransform == null)
            {
                translateTransform = new TranslateTransform();
                SetAdjustTransform(popup, translateTransform);

                if (oldTransform == null)
                {
                    popup.Child.RenderTransform = translateTransform;
                }
                else
                {
                    var group = new TransformGroup();
                    group.Children.Add(oldTransform);
                    group.Children.Add(translateTransform);
                    popup.Child.RenderTransform = group;
                }
            }

            //判断并计算出变换偏移量
            translateTransform.X = 0;
            translateTransform.Y = 0;
            if (bounds.Right > screenBounds.Right)
            {
                translateTransform.X = bounds.Width > screenBounds.Width ? bounds.Right - screenBounds.Right - (bounds.Width - screenBounds.Width) : bounds.Right - screenBounds.Right;
                translateTransform.X += margin.Left + margin.Right;
                if (bounds.Left > screenBounds.Right)
                {
                    translateTransform.X = 0;
                }
            }
            else if (bounds.Left < screenBounds.Left)
            {
                translateTransform.X = bounds.Width > screenBounds.Width ? bounds.Left - screenBounds.Left + (bounds.Width - screenBounds.Width) : bounds.Left - screenBounds.Left;
                if (bounds.Right < screenBounds.Left)
                {
                    translateTransform.X = 0;
                }
            }
            if (bounds.Bottom > screenBounds.Bottom)
            {
                translateTransform.Y = bounds.Height > screenBounds.Height ? bounds.Bottom - screenBounds.Bottom - (bounds.Height - screenBounds.Height) : bounds.Bottom - screenBounds.Bottom;
                translateTransform.Y += margin.Top + margin.Bottom;
                if (bounds.Top > screenBounds.Bottom)
                {
                    translateTransform.Y = 0;
                }
            }
            else if (bounds.Top < screenBounds.Top)
            {
                translateTransform.Y = bounds.Height > screenBounds.Height ? bounds.Top - screenBounds.Top + (bounds.Height - screenBounds.Height) : bounds.Top - screenBounds.Top;
                if (bounds.Bottom < screenBounds.Top)
                {
                    translateTransform.Y = 0;
                }
            }
        }

        private static Matrix GetTransformFromDevice(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source?.CompositionTarget != null) return source.CompositionTarget.TransformFromDevice;
            return Matrix.Identity;
        }
    }

    public enum AlignType
    {
        BottomLeft,
        BottomCenter,
        BottomRight,
        TopLeft,
        TopCenter,
        TopRight,

        LeftBottom,
        LeftCenter,
        LeftTop,
        RightTop,
        RightCenter,
        RightBottom
    }

    [MarkupExtensionReturnType(typeof(CustomPopupPlacementCallback))]
    public class PlacementExtension : MarkupExtension
    {
        [ConstructorArgument("align")]
        public AlignType Align { get; set; }

        [ConstructorArgument("outOfScreenEnabled")]
        public bool OutOfScreenEnabled { get; set; }

        public PlacementExtension()
        {

        }

        public PlacementExtension(AlignType align, bool outOfScreenEnabled)
        {
            Align = align;
            OutOfScreenEnabled = outOfScreenEnabled;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            //这里解决Popup放在Template时无法拿到正确的Popup问题，applyTemplate后方可拿到
            if (provideValueTarget != null && provideValueTarget.TargetObject.GetType().FullName == "System.Windows.SharedDp")
            {
                return this;
            }

            var popup = provideValueTarget?.TargetObject as Popup;
            if (popup == null) return null;

            switch (Align)
            {
                case AlignType.LeftBottom:
                    {
                        return popup.LeftBottom(OutOfScreenEnabled);
                    }
                case AlignType.LeftCenter:
                    {
                        return popup.LeftCenter(OutOfScreenEnabled);
                    }
                case AlignType.LeftTop:
                    {
                        return popup.LeftTop(OutOfScreenEnabled);
                    }
                case AlignType.RightTop:
                    {
                        return popup.RightTop(OutOfScreenEnabled);
                    }
                case AlignType.RightCenter:
                    {
                        return popup.RightCenter(OutOfScreenEnabled);
                    }
                case AlignType.RightBottom:
                    {
                        return popup.RightBottom(OutOfScreenEnabled);
                    }
                case AlignType.BottomLeft:
                    {
                        return popup.BottomLeft(OutOfScreenEnabled);
                    }
                case AlignType.BottomCenter:
                    {
                        return popup.BottomCenter(OutOfScreenEnabled);
                    }
                case AlignType.BottomRight:
                    {
                        return popup.BottomRight(OutOfScreenEnabled);
                    }
                case AlignType.TopLeft:
                    {
                        return popup.TopLeft(OutOfScreenEnabled);
                    }
                case AlignType.TopCenter:
                    {
                        return popup.TopCenter(OutOfScreenEnabled);
                    }
                case AlignType.TopRight:
                    {
                        return popup.TopRight(OutOfScreenEnabled);
                    }
                default:
                    return popup.BottomLeft(OutOfScreenEnabled);
            }
        }
    }
}
