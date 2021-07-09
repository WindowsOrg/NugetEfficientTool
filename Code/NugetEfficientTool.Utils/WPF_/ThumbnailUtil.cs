using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Shell;

namespace NugetEfficientTool.Utils
{
    public static class ThumbnailUtil
    {
        #region 获取系统图标
        private static readonly Dictionary<string, BitmapSource> SmallIconDict = new Dictionary<string, BitmapSource>();
        private static readonly Dictionary<string, BitmapSource> LargeIconDict = new Dictionary<string, BitmapSource>();
        private const string FolderKey = "Folder";
        private const string UndefinedFileKey = "Undefined";
        private static readonly string DefaultFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        /// <summary>
        /// 获取小图标
        /// </summary>
        /// <param name="filePath">对象路径</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static BitmapSource GetSmallIcon(string filePath)
        {
            string key;
            if (IsFolder(filePath))
            {
                filePath = DefaultFolderPath;
                key = FolderKey;
            }
            else
            {
                var extension = Path.GetExtension(filePath);
                key = string.IsNullOrWhiteSpace(extension) ? UndefinedFileKey : extension;
            }

            if (SmallIconDict.ContainsKey(key)) return SmallIconDict[key];

            try
            {
                using (var sf = ShellObject.FromParsingName(filePath))
                {
                    sf.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                    if (key.EndsWith(".exe") || key.EndsWith(".lnk"))
                    {
                        return sf.Thumbnail.SmallBitmapSource;
                    }
                    SmallIconDict[key] = sf.Thumbnail.SmallBitmapSource;
                    return SmallIconDict[key];
                }
            }

            // NotSupportedException; InvalidOperationException;ShellException;AccessViolationException(不一定能捕捉到)
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// 获取大图标
        /// </summary>
        /// <param name="filePath">对象路径</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static BitmapSource GetLargeIcon(string filePath)
        {
            string key;
            if (IsFolder(filePath))
            {
                filePath = DefaultFolderPath;
                key = FolderKey;
            }
            else
            {
                var extension = Path.GetExtension(filePath);
                key = string.IsNullOrWhiteSpace(extension) ? UndefinedFileKey : extension;
            }

            if (LargeIconDict.ContainsKey(key)) return LargeIconDict[key];
            try
            {
                using (var sf = ShellObject.FromParsingName(filePath))
                {
                    sf.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                    if (key.EndsWith(".exe") || key.EndsWith(".lnk"))
                    {
                        return sf.Thumbnail.ExtraLargeBitmapSource;
                    }
                    LargeIconDict[key] = sf.Thumbnail.ExtraLargeBitmapSource;
                    return LargeIconDict[key];
                }
            }
            // NotSupportedException; InvalidOperationException;ShellException;AccessViolationException(不一定能捕捉到)
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取缩略图
        /// </summary>
        /// <param name="filePath">对象路径</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public static BitmapSource GetThumbnail(string filePath)
        {
            //WindowsApiCodePack
            try
            {
                using (var sf = ShellObject.FromParsingName(filePath))
                {
                    sf.Thumbnail.FormatOption = ShellThumbnailFormatOption.Default;
                    return sf.Thumbnail.MediumBitmapSource;
                }
            }
            // NotSupportedException; InvalidOperationException;ShellException;AccessViolationException(不一定能捕捉到)
            catch
            {
                return null;
            }
        }

        #endregion

        private static readonly Lazy<FieldInfo> Field = new Lazy<FieldInfo>(() => typeof(DispatcherObject).GetField("_dispatcher",
          BindingFlags.Instance | BindingFlags.NonPublic));

        /// <summary>
        /// 更换线程调度器，解决线程间资源共享问题
        /// </summary>
        /// <param name="obj">待切换线程的对象</param>
        /// <param name="newDispatcher">切换到的线程</param>
        public static void ReplaceDispatcher(DispatcherObject obj, Dispatcher newDispatcher)
        {
            if (obj == null || obj.Dispatcher == newDispatcher) return;
            Field.Value.SetValue(obj, newDispatcher);
        }

        /// <summary>
        /// 判断是否为文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }
    }
}
