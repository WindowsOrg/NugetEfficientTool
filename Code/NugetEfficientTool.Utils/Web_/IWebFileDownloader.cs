using System;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public interface IWebFileDownloader
    {
        Task<(bool success, string downloadTempPath, Exception exception)> DownloadFileAsync(string resourceUri, string extension = "");
    }
}
