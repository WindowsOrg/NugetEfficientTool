using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// 引用方式检查器
    /// </summary>
    public class ReferenceWayChecker
    {
        private readonly string _solutionFile;

        public ReferenceWayChecker(string solutionFile)
        {
            _solutionFile = solutionFile;
        }
        public void Check()
        {
            var folder = _solutionFile;
            if (_solutionFile.EndsWith(".sln") && !File.Exists(_solutionFile))
            {
                folder = Path.GetDirectoryName(folder);
            }
            if (!Directory.Exists(folder))
            {
                return;
            }
            var csProjFiles = Directory.GetFiles(folder, CustomText.CsProjSearchPattern,SearchOption.AllDirectories);
            foreach (var csProjFile in csProjFiles)
            {
                var formatUpdater = new CsProjReferenceWayUpdater(csProjFile);
                if (formatUpdater.CanUpgrade())
                {
                    NeedFix = true;
                    break;
                }
            }
            var packageFiles = Directory.GetFiles(folder, CustomText.PackagesConfigSearchPattern, SearchOption.AllDirectories);
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
