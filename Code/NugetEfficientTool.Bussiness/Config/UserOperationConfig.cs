using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotnetCampus.Configurations;
using dotnetCampus.Configurations.Core;

namespace NugetEfficientTool.Business
{
    public class UserOperationConfig
    {
        private readonly DefaultConfiguration _configs;
        public UserOperationConfig()
        {
            var configFilePath = CustomText.Path.ConfigFilePath;
            _configs = ConfigurationFactory.FromFile(configFilePath).CreateAppConfigurator().Of<DefaultConfiguration>();
        }

        public string GetSolutionFile()
        {
           return _configs["SolutionFile"] ?? string.Empty;
        }

        public void SaveSolutionFile(string solutionFile)
        {
            _configs["SolutionFile"] = solutionFile;
        }
    }
}
