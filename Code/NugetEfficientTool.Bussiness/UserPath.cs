using System;
using System.IO;

namespace NugetEfficientTool.Business
{
    public class UserPath
    {
        public string AppDataFolder => _appDataFolder ??= GetAppDataFolder();
        private string GetAppDataFolder()
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataFolder = Path.Combine(folderPath,CustomText.ProjectName);
            EnsureExistFolder(appDataFolder);
            return appDataFolder;
        }
        public string ConfigFilePath => Path.Combine(AppDataFolder, "Configs.fkv");

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
