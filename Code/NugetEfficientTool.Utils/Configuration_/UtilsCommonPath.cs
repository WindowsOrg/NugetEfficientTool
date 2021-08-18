using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace NugetEfficientTool.Utils
{
    public static class UtilsCommonPath
    {
        public static string GetDownloadFolder()
        {
            var downloadFolder = Path.Combine(GetAppDataFolder(), "Download");
            downloadFolder = EnsureDirectory(downloadFolder);
            return downloadFolder;
        }
        public static string GetLogFolder()
        {
            string appdataPath = GetAppDataFolder();
            string logFilePath = Path.Combine(appdataPath, "Log");

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }
            return logFilePath;
        }

        private static string _appDataFolder = string.Empty;
        public static string GetAppDataFolder(string projectName="")
        {
            if (string.IsNullOrEmpty(_appDataFolder))
            {
                var appName = projectName;
                if (string.IsNullOrWhiteSpace(appName))
                {
                    var canGetAppName=TryGetAppName(out appName);
                    if (!canGetAppName)
                    {
                        var baseDirectory = Directory.GetCurrentDirectory();
                        appName = new DirectoryInfo(baseDirectory).Name;
                    }
                }
                var dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                _appDataFolder = Path.Combine(dataFolder,appName);
            }

            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }
            return _appDataFolder;
        }

        private static bool TryGetAppName(out string appName)
        {
            appName = string.Empty;
            try
            {
                var baseDirectory = GetExeRunFolder();
                string startupUri = Application.Current.StartupUri?.AbsolutePath;
                appName = Path.GetFileNameWithoutExtension(startupUri);
                if (string.IsNullOrEmpty(appName))
                {
                    var exeFile = Directory.GetFiles(baseDirectory, "*.exe").FirstOrDefault();
                    if (!string.IsNullOrEmpty(exeFile))
                    {
                        appName = Path.GetFileNameWithoutExtension(exeFile);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return !string.IsNullOrEmpty(appName);
        }

        public static void SetAppDataFolder(string appName)
        {
            var dataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _appDataFolder = Path.Combine(dataFolder, appName);
            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }
        }

        public static string GetExeRunFolder()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return baseDirectory;
        }

        public static string EnsureDirectory(string folder)
        {
            FolderHelper.CreateFolder(folder);
            return folder;
        }

        public static string GetDesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
    }
}
