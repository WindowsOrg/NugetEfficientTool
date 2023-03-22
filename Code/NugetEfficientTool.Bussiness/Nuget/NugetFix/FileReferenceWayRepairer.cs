using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NugetEfficientTool.Business
{
    /// <summary>
    /// Nuget引用方式修复器
    /// </summary>
    public class FileReferenceWayRepairer
    {
        private readonly string _solutionFile;

        public FileReferenceWayRepairer(string solutionFile)
        {
            _solutionFile = solutionFile;
        }

        public void Fix()
        {
            var folder = _solutionFile;
            if (string.IsNullOrEmpty(folder))
            {
                return;
            }
            if (_solutionFile.EndsWith(".sln") && !File.Exists(_solutionFile))
            {
                folder = Path.GetDirectoryName(folder);
            }
            if (!Directory.Exists(folder))
            {
                return;
            }
            var csProjFiles = Directory.GetFiles(folder, CustomText.CsProjSearchPattern, SearchOption.AllDirectories);
            foreach (var csProjFile in csProjFiles)
            {
                var formatUpdater = new CsProjReferenceWayUpdater(csProjFile);
                if (formatUpdater.TryUpgrade() && !string.IsNullOrEmpty(formatUpdater.Log))
                {
                    var headerMessage = string.Format(CustomText.FixSuccessKey, csProjFile);
                    Log = StringSplicer.SpliceWithDoubleNewLine(Log, headerMessage);
                    Log = StringSplicer.SpliceWithNewLine(Log, formatUpdater.Log);
                }
            }
            var packageFiles = Directory.GetFiles(folder, CustomText.PackagesConfigSearchPattern, SearchOption.AllDirectories);
            foreach (var packageFile in packageFiles)
            {
                var packageUpdater = new PackageConfigReferenceWayUpdater(packageFile);
                if (packageUpdater.TryUpgrade() && !string.IsNullOrEmpty(packageUpdater.Log))
                {
                    var headerMessage = string.Format(CustomText.FixSuccessKey, packageFile);
                    Log = StringSplicer.SpliceWithDoubleNewLine(Log, headerMessage);
                    Log = StringSplicer.SpliceWithNewLine(Log, packageUpdater.Log);
                }
            }
        }
        public string Log { get; set; }
    }
}
