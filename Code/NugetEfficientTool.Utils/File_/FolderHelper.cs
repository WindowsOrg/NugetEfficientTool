using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace NugetEfficientTool.Utils
{
    public static class FolderHelper
    {
        public static void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }
        public static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public static List<string> GetAllFiles(string folder, List<string> supportFileTypes)
        {
            var fileList = new List<string>();
            if (!Directory.Exists(folder) || supportFileTypes == null || supportFileTypes.Count == 0)
            {
                return fileList;
            }
            fileList = supportFileTypes.SelectMany(i => Directory.GetFiles(folder, i, SearchOption.AllDirectories)).ToList();
            return fileList;
        }
        /// <summary>
        /// 获取文件夹内文件
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="supportFileTypes"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string folder, List<string> supportFileTypes)
        {
            var fileList = new List<string>();
            if (!Directory.Exists(folder) || supportFileTypes == null || supportFileTypes.Count == 0)
            {
                return fileList;
            }
            fileList = supportFileTypes.SelectMany(i => Directory.GetFiles(folder, i, SearchOption.TopDirectoryOnly)).ToList();
            //去重 *.ppt *.pptx后缀,对pptx文件会多次筛选
            fileList = fileList.Distinct().ToList();
            return fileList;
        }
        /// <summary>
        /// 从指定目录中获取所有文件（含子文件夹）
        /// </summary>
        /// <param name="folder">待遍历的目录</param>
        /// <param name="searchPattern">搜索字符串</param>
        /// <returns>获取到的文件路径列表</returns>
        public static IEnumerable<string> GetFilesFromDirectory(string folder, string searchPattern = null)
        {
            if (!Directory.Exists(folder))
            {
                return new List<string>();
            }
            searchPattern ??= "*";

            var files = Directory.EnumerateFiles(folder, searchPattern);
            foreach (var directory in Directory.GetDirectories(folder))
            {
                files = files.Concat(GetFilesFromDirectory(directory, searchPattern));
            }

            return files;
        }
    }
}
