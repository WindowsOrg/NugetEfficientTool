using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NugetEfficientTool.Utils
{
    public class WebRequestBase
    {
        private const string EmptyJsonData = "{}";

        #region Request

        /// <summary>
        /// Get by webRequest
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="headersDict"></param>
        /// <returns></returns>
        protected virtual async Task<TResponse> GetAsync<TResponse>(string url, HttpRequest request = null, Dictionary<string, string> headersDict = null)
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "get";
            webRequest.ContentType = "application/json";
            if (headersDict != null)
            {
                foreach (var headerTuple in headersDict)
                {
                    webRequest.Headers.Add(headerTuple.Key, headerTuple.Value);
                }
            }

            if (request != null)
            {
                var jsonData = JsonConvert.SerializeObject(request);
                if (!string.IsNullOrWhiteSpace(jsonData) && jsonData != EmptyJsonData)
                {
                    byte[] databyte = Encoding.UTF8.GetBytes(jsonData);
                    webRequest.ContentLength = databyte.Length;
                    using (Stream requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(databyte, 0, databyte.Length);
                    }
                }
            }

            var response = await webRequest.GetResponseAsync();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream ?? throw new InvalidOperationException(),
                    Encoding.GetEncoding("utf-8")))
                {
                    string result = reader.ReadToEnd();
                    var decodeResult = UnicodeHelper.Unicode2String(result);
                    var dataResponse = JsonConvert.DeserializeObject<TResponse>(decodeResult);
                    return dataResponse;
                }
            }
        }

        #endregion

        #region Post

        /// <summary>
        /// Posting by WebRequest
        /// </summary>
        /// <typeparam name="TReponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <param name="headersDict"></param>
        /// <returns></returns>
        protected virtual async Task<(TReponse response, string decodeResult, WebHeaderCollection Headers)>
            PostAsync<TReponse>(string url, HttpRequest request, Dictionary<string, string> headersDict = null)
        {
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "post";
            webRequest.ContentType = "application/json;charset=utf-8";
            if (headersDict != null)
            {
                foreach (var headerTuple in headersDict)
                {
                    webRequest.Headers.Add(headerTuple.Key, headerTuple.Value);
                }
            }

            var jsonData = JsonConvert.SerializeObject(request);
            if (!string.IsNullOrWhiteSpace(jsonData) && jsonData != EmptyJsonData)
            {
                byte[] postdatabyte = Encoding.UTF8.GetBytes(jsonData);
                webRequest.ContentLength = postdatabyte.Length;
                using (Stream postStream = webRequest.GetRequestStream())
                {
                    postStream.Write(postdatabyte, 0, postdatabyte.Length);
                }
            }

            var response = await webRequest.GetResponseAsync();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream ?? throw new InvalidOperationException(),
                    Encoding.GetEncoding("utf-8")))
                {
                    string result = reader.ReadToEnd();
                    var decodeResult = UnicodeHelper.Unicode2String(result);
                    var dataResponse = JsonConvert.DeserializeObject<TReponse>(decodeResult);
                    return (dataResponse,decodeResult, response.Headers);
                }
            }
        }

        /// <summary>
        /// Post using HttpClient
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="request"></param>
        /// <param name="headersDict"></param>
        /// <returns></returns>
        protected virtual async Task<TReponse> PostByHttpAsync<TReponse, TRequest>(string requestUrl, HttpRequest request, Dictionary<string, string> headersDict = null)
        {
            var jsonData = JsonConvert.SerializeObject(request);
            HttpContent httpContent = new StringContent(jsonData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            if (headersDict != null)
            {
                foreach (var headerTuple in headersDict)
                {
                    httpContent.Headers.Add(headerTuple.Key, headerTuple.Value);
                }
            }

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage response = httpClient.PostAsync(requestUrl, httpContent).Result)
                {
                    if (response.IsSuccessStatusCode && response.Content != null)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var dataResponse = JsonConvert.DeserializeObject<TReponse>(result);
                        return dataResponse;
                    }
                }
            }
            return default(TReponse);
        }

        protected virtual async Task<string>
            PostDownloadAsync(string url, HttpRequest request, Dictionary<string, string> headersDict = null, string extension = null)
        {
            //解决“请求被中止: 未能创建 SSL/TLS 安全通道”的问题
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = "post";
            webRequest.ContentType = "application/json;charset=utf-8";
            if (headersDict != null)
            {
                foreach (var headerTuple in headersDict)
                {
                    webRequest.Headers.Add(headerTuple.Key, headerTuple.Value);
                }
            }
            //参数
            var jsonData = JsonConvert.SerializeObject(request);
            if (!string.IsNullOrWhiteSpace(jsonData) && jsonData != EmptyJsonData)
            {
                byte[] postdatabyte = Encoding.UTF8.GetBytes(jsonData);
                webRequest.ContentLength = postdatabyte.Length;
                using (Stream postStream = webRequest.GetRequestStream())
                {
                    postStream.Write(postdatabyte, 0, postdatabyte.Length);
                }
            }
            //发送请求并获取相应回应数据
            using (var response = await webRequest.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    //获取文件后缀
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = new WebRequestFileDownloader().GetFileExtension(url, response.Headers);
                    }
                    if (string.IsNullOrEmpty(extension))
                    {
                        return string.Empty;
                    }
                    var userDownloadFolder = UtilsCommonPath.GetDownloadFolder();
                    var downloadPath = Path.Combine(userDownloadFolder, $"{Guid.NewGuid()}{extension}");
                    using (FileStream writer = File.Create(downloadPath))
                    {
                        byte[] buff = new byte[1024];
                        int c = 0;
                        //实际读取的字节数   
                        while ((c = (responseStream ?? throw new InvalidOperationException($"PostDownloadAsync：{url},{nameof(responseStream)}创建失败！")).Read(buff, 0, buff.Length)) > 0)
                        {
                            writer.Write(buff, 0, c);
                        }
                        writer.Flush(true);
                        response.Close();
                    }
                    return downloadPath;
                }
            }
        }

        #endregion

        #region PostFormData

        /// <summary>
        /// 发送表单数据
        /// </summary>
        /// <typeparam name="TReponse"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="formDatas"></param>
        /// <param name="userToken"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public async Task<TReponse> PostFormByHttpAsync<TReponse>(string requestUri, List<PostFormItem> formDatas = null,
        string userToken = "", int timeOut = 30000)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
            if (!string.IsNullOrWhiteSpace(userToken))
            {
                requestMessage.Headers.TryAddWithoutValidation("x-oauth-token", userToken);
            }

            //添加Form数据 - 字段/文件参数
            if (formDatas != null)
            {
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                //1.普通参数
                var formDataList = formDatas.Where(i => !i.IsFile).ToList();
                foreach (var formData in formDataList)
                {
                    multipartFormDataContent.Add(new StringContent(formData.Value), formData.Key);
                }
                //2.文件参数
                var formFileList = formDatas.Where(i => i.IsFile).ToList();
                int index = 0;
                foreach (var formFile in formFileList)
                {
                    var fileKey = string.IsNullOrWhiteSpace(formFile.Key)
                        ? (formFileList.Count > 0 ? $"file{index++}" : "file")
                        : formFile.Key;
                    multipartFormDataContent.Add(new ByteArrayContent(File.ReadAllBytes(formFile.Value)), fileKey,
                        HttpUtility.UrlEncode(formFile.FileName));
                }
                requestMessage.Content = multipartFormDataContent;
            }
            //发送
            using (var httpClient = new HttpClient()
            {
                MaxResponseContentBufferSize = 2147483647
            })
            {
                var response = await httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var dataResponse = JsonConvert.DeserializeObject<TReponse>(result);
                    return dataResponse;
                }
            }
            return default(TReponse);
        }

        #endregion

    }

    /// <summary>
    /// 表单数据项
    /// </summary>
    public class PostFormItem
    {
        public PostFormItem(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public PostFormItem(string key, string value, bool isFile) : this(key, value)
        {
            IsFile = isFile && !string.IsNullOrWhiteSpace(value) && File.Exists(value);
        }

        /// <summary>
        /// 表单键，request["key"]
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 表单值,request["key"].value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 是否是文件
        /// </summary>
        public bool IsFile { get; }

        /// <summary>
        /// 上传的文件名
        /// </summary>
        public string FileName => IsFile ? Path.GetFileName(Value) : string.Empty;
    }
}
