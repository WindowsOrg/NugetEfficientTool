﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetEfficientTool.Nuget;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget组合修复
    /// </summary>
    public class NugetMismatchVersionGroupFix
    {
        private readonly IEnumerable<FileNugetInfoGroup> _mismatchVersionNugets;
        private readonly List<NugetFixStrategy> _nugetFixStrategies;

        public NugetMismatchVersionGroupFix(IEnumerable<FileNugetInfoGroup> mismatchVersionNugets, List<NugetFixStrategy> nugetFixStrategies)
        {
            _mismatchVersionNugets = mismatchVersionNugets;
            _nugetFixStrategies = nugetFixStrategies;
        }

        public string Fix()
        {
            var repairLog = string.Empty;
            var toReparingFiles = new List<string>();
            var fileNugetInfos = _mismatchVersionNugets.SelectMany(i => i.FileNugetInfos);
            foreach (var nugetFile in fileNugetInfos)
            {
                if (_nugetFixStrategies.All(i => i.NugetName != nugetFile.Name))
                {
                    continue;
                }

                //如果文件已经满足当前修复策略，则跳过
                if (_nugetFixStrategies.All(i => $"{i.NugetName}_{i.NugetVersion}" ==
                                                 $"{nugetFile.Name}_{nugetFile.Version}"))
                {
                    continue;
                }

                //跳过已记录和空文件
                var fileConfigPath = nugetFile.ConfigPath;
                if (toReparingFiles.Any(i => i == fileConfigPath) || nugetFile.IsEmptyFile)
                {
                    continue;
                }

                toReparingFiles.Add(fileConfigPath);
            }

            //对文件进行修复
            foreach (var configFile in toReparingFiles)
            {
                var nugetConfigRepairer = new FileNugetVersionRepairer(configFile, _nugetFixStrategies);
                nugetConfigRepairer.Repair();
                if (!string.IsNullOrEmpty(nugetConfigRepairer.Log))
                {
                    repairLog = StringSplicer.SpliceWithDoubleNewLine(repairLog, nugetConfigRepairer.Log);
                }
            }

            return repairLog;
        }
    }
}
