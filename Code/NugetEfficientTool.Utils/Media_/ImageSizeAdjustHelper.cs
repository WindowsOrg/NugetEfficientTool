using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace NugetEfficientTool.Utils
{
    public class ImageSizeAdjustHelper
    {
        #region 限制图片最大高宽显示

        /// <summary>
        /// 限制图片最大高宽显示
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        public static void AdjustImageByMaxSize(string imageFilePath, int maxWidth, int maxHeight)
        {
            AdjustImageByMaxSize(imageFilePath, maxWidth, maxHeight, InterpolationMode.Default);
        }
        public static void AdjustImageByMaxSize(string imageFilePath, int maxWidth, int maxHeight, InterpolationMode interpolationMode)
        {
            using (Bitmap newImage = new Bitmap(maxWidth, maxHeight, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)newImage))
                {
                    g.InterpolationMode = interpolationMode;
                    using (Image image = Image.FromFile(imageFilePath))
                    {
                        var adjustedSize = GetAllowedMaxSize(image.Width, image.Height, maxWidth, maxHeight);
                        int x = (maxWidth - adjustedSize.adjustedWidth) / 2;
                        int y = (maxHeight - adjustedSize.adjustedHeight) / 2;
                        g.DrawImage(image, x, y, adjustedSize.adjustedWidth, adjustedSize.adjustedHeight);
                    }
                }
                File.Delete(imageFilePath);
                newImage.Save(imageFilePath, ImageFormat.Png);
            }
        }
        /// <summary>
        /// 获取可容许的最大尺寸
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        private static (int adjustedWidth, int adjustedHeight) GetAllowedMaxSize(int imageWidth, int imageHeight, int maxWidth, int maxHeight)
        {
            var adjustedWidth = imageWidth;
            var adjustedHeight = imageHeight;
            var sourceRatio = adjustedHeight / Convert.ToDouble(adjustedWidth);
            if (adjustedWidth > maxWidth)
            {
                adjustedWidth = maxWidth;
                adjustedHeight = Convert.ToInt32(sourceRatio * adjustedWidth);
            }

            if (adjustedHeight > maxHeight)
            {
                adjustedHeight = maxHeight;
                adjustedWidth = Convert.ToInt32(adjustedHeight / sourceRatio);
            }

            return (adjustedWidth, adjustedHeight);
        }

        #endregion


        #region 固定图片高宽显示

        /// <summary>
        /// 固定图片高宽显示
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        public static void AdjustImageByFixedSize(string imageFilePath, int fixedWidth, int fixedHeight)
        {
            AdjustImageByFixedSize(imageFilePath, fixedWidth, fixedHeight, InterpolationMode.Default);
        }
        public static void AdjustImageByFixedSize(string imageFilePath, int fixedWidth, int fixedHeight, InterpolationMode interpolationMode)
        {
            using (Bitmap newImage = new Bitmap(fixedWidth, fixedHeight, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage((System.Drawing.Image)newImage))
                {
                    g.Clear(Color.Black);
                    g.InterpolationMode = interpolationMode;
                    using (Image image = Image.FromFile(imageFilePath))
                    {
                        var adjustedSize = GetAllowedFixedSize(image.Width, image.Height, fixedWidth, fixedHeight);
                        int x = (fixedWidth - adjustedSize.adjustedWidth) / 2;
                        int y = (fixedHeight - adjustedSize.adjustedHeight) / 2;
                        g.DrawImage(image, x, y, adjustedSize.adjustedWidth, adjustedSize.adjustedHeight);
                    }
                }
                File.Delete(imageFilePath);
                newImage.Save(imageFilePath, ImageFormat.Png);
            }
        }
        /// <summary>
        /// 获取可容许的固定尺寸
        /// </summary>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="fixedWidth"></param>
        /// <param name="fixedHeight"></param>
        /// <returns></returns>
        private static (int adjustedWidth, int adjustedHeight) GetAllowedFixedSize(int imageWidth, int imageHeight, int fixedWidth, int fixedHeight)
        {
            var adjustedWidth = imageWidth;
            var adjustedHeight = imageHeight;
            //高宽比
            var sourceRatio = adjustedHeight / Convert.ToDouble(adjustedWidth);
            var fixedRatio = fixedHeight / Convert.ToDouble(fixedWidth);
            //超出显示时，压缩
            //高宽均小于固定尺寸时，扩大比例显示
            if (adjustedWidth > fixedWidth || adjustedHeight > fixedHeight
                || (adjustedWidth < fixedWidth && adjustedHeight < fixedHeight))
            {
                //原图片宽度比例大时，扩大宽度
                if (sourceRatio <= fixedRatio)
                {
                    adjustedWidth = fixedWidth;
                    adjustedHeight = Convert.ToInt32(sourceRatio * fixedWidth);
                }
                //原图片高度比例大时，扩大高度
                else
                {
                    adjustedHeight = fixedHeight;
                    adjustedWidth = Convert.ToInt32(fixedHeight / sourceRatio);
                }
            }

            return (adjustedWidth, adjustedHeight);
        }

        #endregion

    }
}
