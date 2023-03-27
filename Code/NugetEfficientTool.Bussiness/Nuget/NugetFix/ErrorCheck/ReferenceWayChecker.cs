using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 引用方式检查器
    /// </summary>
    public class ReferenceWayChecker
    {
        private readonly List<string> _solutionFiles;

        public ReferenceWayChecker(List<string> solutionFiles)
        {
            if (solutionFiles.Count == 0)
            {
                throw new ArgumentNullException(nameof(solutionFiles));
            }
            _solutionFiles = solutionFiles;
        }
        public void Check()
        {
            var solutionFiles = _solutionFiles;
            var solutionFolders = solutionFiles.Select(Path.GetDirectoryName).ToList();
            //Csproj文件
            var csProjFiles = solutionFolders.SelectMany(i => FolderHelper.GetFilesFromDirectory(i, CustomText.CsProjSearchPattern));
            foreach (var csProjFile in csProjFiles)
            {
                var formatUpdater = new CsProjReferenceWayUpdater(csProjFile);
                if (formatUpdater.CanUpgrade())
                {
                    NeedFix = true;
                    break;
                }
            }
            //package.config文件
            var packageFiles = solutionFolders.SelectMany(i => FolderHelper.GetFilesFromDirectory(i, CustomText.PackagesConfigSearchPattern));
            foreach (var packageFile in packageFiles)
            {
                var formatUpdater = new PackageConfigReferenceWayUpdater(packageFile);
                if (formatUpdater.CanUpgrade())
                {
                    NeedFix = true;
                    break;
                }
            }
        }
        public bool NeedFix { get; private set; }
    }
}
