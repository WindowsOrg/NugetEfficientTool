namespace NugetEfficientTool.Nuget
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
        public static List<string> GetAllFiles(string folder, string searchPattern = null)
        {
            return GetAllFiles(folder, new List<string>() { searchPattern ?? "*" });
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
        public static List<string> GetFiles(string folder, string searchPattern = null)
        {
            return GetFiles(folder, new List<string>() { searchPattern ?? "*" });
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
    }
}
