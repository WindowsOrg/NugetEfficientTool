using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NugetEfficientTool.Utils
{
    public static class LogHelper
    {
        public static void LogInfo(string message)
        {
            try
            {
                var infos=new List<string>()
                {
                    $"{DateTime.Now:yyyy-MM-dd hh:mm:ss ffff} {message}"
                };
                string logFilePath = GetLogInfoPath();
                File.AppendAllLines(logFilePath, infos);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public static void LogError(string message)
        {
            try
            {
                var infos = new List<string>()
                {
                    "*********************************************************************\r\n",
                    $"记录时间:{DateTime.Now:yyyy-MM-dd hh:mm:ss ffff}",
                    $"错误描述:{message}\r\n",
                };
                string logFilePath = GetLogErrorPath();
                File.AppendAllLines(logFilePath, infos);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public static void LogError(string message,Exception ex)
        {
            try
            {
                var infos = new List<string>()
                {
                    "*********************************************************************\r\n",
                    $"记录时间:{DateTime.Now:yyyy-MM-dd hh:mm:ss ffff}",
                    $"错误描述:{message}",
                    $"{ex.Message}",
                    $"{ex.StackTrace}\r\n",
                };
                string logFilePath = GetLogErrorPath();
                File.AppendAllLines(logFilePath, infos);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public static void LogError(Exception ex)
        {
            try
            {
                var infos = new List<string>()
                {
                    "*********************************************************************\r\n",
                    $"记录时间:{DateTime.Now:yyyy-MM-dd hh:mm:ss ffff}",
                    $"{ex.Message}",
                    $"{ex.StackTrace}\r\n",
                };
                string logFilePath = GetLogErrorPath();
                File.AppendAllLines(logFilePath, infos);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void SetProjectName(string projectName)
        {
            _projectName = projectName;
        }
        private static string _logAppDataFolder = string.Empty;
        public static string GetLogFolder()
        {
            if (string.IsNullOrEmpty(_logAppDataFolder))
            {
                if (string.IsNullOrWhiteSpace(_projectName))
                {
                    return UtilsCommonPath.GetLogFolder();
                }
                string appdataPath = UtilsCommonPath.GetAppDataFolder(_projectName);
                _logAppDataFolder = Path.Combine(appdataPath, "Log");
            }
            if (!Directory.Exists(_logAppDataFolder))
            {
                Directory.CreateDirectory(_logAppDataFolder);
            }
            return _logAppDataFolder;
        }

        #region private
        
        private static string GetLogInfoPath()
        {
            return GetLogInfoFilePath($"info_{DateTime.Now:yyyy-MM-dd}.txt");
        }
        private static string GetLogErrorPath()
        {
            return GetLogInfoFilePath($"error_{DateTime.Now:yyyy-MM-dd}.txt");
        }

        private static string GetLogInfoFilePath(string fileName)
        {
            string logFolder= GetLogFolder();
            string logFilePath = Path.Combine(logFolder, fileName);

            try
            {
                if (!File.Exists(logFilePath))
                {
                    var aaa = File.Create(logFilePath);
                    aaa.Dispose();
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return logFilePath;
        }

        private static string _projectName = string.Empty;

        #endregion
    }
}
