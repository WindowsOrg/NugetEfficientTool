using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public class WebRequestFileDownloader:IWebFileDownloader
    {
        /// <summary>
        /// 下载文件（自动获取文件后缀，如获取不到则下载失败）
        /// </summary>
        /// <param name="resourceUri">
        /// <remarks>包含后缀的文件地址:"http://ydschool-online.nos.netease.com/account_v0205.mp3"</remarks><br/>
        /// <remarks>不包含后缀的文件地址（返回response的Content-Disposition含有文件后缀）:"http://ydschool-online.nos.netease.com/account_v0205.mp3"</remarks>
        /// </param>
        /// <returns></returns>
        public async Task<(bool success, string downloadTempPath,Exception exception)> DownloadFileAsync(string resourceUri,string extension="")
        {
            if (string.IsNullOrWhiteSpace(resourceUri))
            {
                return (false,string.Empty,new ArgumentException($"参数{nameof(resourceUri)}不能为空"));
            }

            try
            {
                //C# 解决“请求被中止: 未能创建 SSL/TLS 安全通道”的问题
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12; //加上这一句
                // 设置参数
                WebRequest request = WebRequest.Create(resourceUri);
                //发送请求并获取相应回应数据
                using (var response = await request.GetResponseAsync())
                {
                    using (Stream reader = response.GetResponseStream())
                    {
                        //获取文件后缀
                        if (string.IsNullOrEmpty(extension))
                        {
                            extension =  GetFileExtension(resourceUri, response.Headers);
                        }
                        if (string.IsNullOrEmpty(extension))
                        {
                            return (false, string.Empty, new NotSupportedException($"下载文件{resourceUri}后缀不能为空"));
                        }
                        var userDownloadFolder = UtilsCommonPath.GetDownloadFolder();
                        var downloadPath = Path.Combine(userDownloadFolder, $"{Guid.NewGuid()}{extension}");
                        if (File.Exists(downloadPath))
                        {
                            File.Delete(downloadPath);
                        }
                        using (FileStream writer = File.Create(downloadPath))
                        {
                            byte[] buff = new byte[1024];
                            int c = 0;                                           //实际读取的字节数   
                            while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                            {
                                writer.Write(buff, 0, c);
                            }
                            writer.Flush(true);
                            response.Close();
                        }
                        return (true, downloadPath,null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, string.Empty,ex);
            }
        }

        public string GetFileExtension(string resourceUri, WebHeaderCollection responseHeaderCollection)
        {
            var extension = Path.GetExtension(resourceUri);
            if (string.IsNullOrEmpty(extension)&&
                responseHeaderCollection!=null&& responseHeaderCollection.AllKeys.Any(i=>i== "Content-Disposition"))
            {
                var responseHeader = responseHeaderCollection["Content-Disposition"];
                var headers = responseHeader.Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (headers.Length >= 2 && headers[1].ToLower().Contains("filename") && headers[1].Contains("."))
                {
                    var lastIndexOfPoint = headers[1].LastIndexOf(".", StringComparison.Ordinal);
                    extension = headers[1].Substring(lastIndexOfPoint, headers[1].LastIndexOf("\"", StringComparison.Ordinal) - lastIndexOfPoint);
                }
            }
            return extension;
        }
    }
}
