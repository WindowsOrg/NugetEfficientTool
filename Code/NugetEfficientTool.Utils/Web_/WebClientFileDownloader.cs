using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public class WebClientFileDownloader:IWebFileDownloader
    {
        public async Task<(bool success, string downloadTempPath, Exception exception)> DownloadFileAsync(string resourceUri, string extension="")
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = Path.GetExtension(resourceUri);
                    }
                    if (string.IsNullOrEmpty(extension))
                    {
                        return (false, string.Empty, new NotSupportedException($"下载文件{resourceUri}后缀不能为空"));
                    }
                    var userDownloadFolder = UtilsCommonPath.GetDownloadFolder();
                    var downloadPath = Path.Combine(userDownloadFolder, $"{Guid.NewGuid()}{extension}");
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(resourceUri, downloadPath);
                    }

                    return (true, downloadPath, null);
                }
                catch (Exception e)
                {
                    return (false, string.Empty, e);
                }
            });
        }

        public void DownloadFile(string resourceUri, string extension = "")
        {
            if (string.IsNullOrEmpty(extension))
            {
                extension = Path.GetExtension(resourceUri);
            }
            if (string.IsNullOrEmpty(extension))
            {
                throw new NotSupportedException($"下载文件{resourceUri}后缀不能为空");
            }
            var userDownloadFolder = UtilsCommonPath.GetDownloadFolder();
            var downloadPath = Path.Combine(userDownloadFolder, $"{Guid.NewGuid()}{extension}");
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += OnDownloadProgressChanged;
                webClient.DownloadFileCompleted += OnDownloadFileCompleted;
                webClient.DownloadFileAsync(new Uri(resourceUri), downloadPath);
            }
        }

        public event EventHandler<AsyncCompletedEventArgs> DownloadFileCompleted;
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadFileCompleted?.Invoke(sender,e);
        }
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(sender,e);
        }
    }
}
