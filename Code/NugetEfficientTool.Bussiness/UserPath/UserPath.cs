using System;
using System.IO;

namespace NugetEfficientTool.Business
{
    public class UserPath
    {
        public string ConfigFilePath => GetConfigFilePath();

        private string GetConfigFilePath()
        {
            var configPath = Path.Combine(AppDataFolder, "Configs.fkv");
            return configPath;
        }
        public string AppDataFolder => _appDataFolder ?? (_appDataFolder = GetAppDataFolder());
        private string GetAppDataFolder()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(folderPath,CustomText.ProjectName);
            EnsureExistFolder(appDataFolder);
            return appDataFolder;
        }

        private void EnsureExistFolder(string folder)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private string _appDataFolder;
    }
}
