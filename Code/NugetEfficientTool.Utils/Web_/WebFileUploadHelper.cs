using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    public class WebFileUploadHelper
    {
        private static readonly Encoding DefaultEncode = Encoding.UTF8;

        #region UplodFile

        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string UploadFile(string url, string file)
        {
            return UploadFile(url, file, null, DefaultEncode);
        }
        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string UploadFile(string url, string file, NameValueCollection data)
        {
            return UploadFile(url, file, data, DefaultEncode);
        }

        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UploadFile(string url, string file, NameValueCollection data, Encoding encoding)
        {
            return UploadFiles(url, new string[] { file }, data, encoding);
        }
        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string UploadFiles(string url, string file, NameValueCollection data)
        {
            return UploadFiles(url, new string[] { file }, data, DefaultEncode);
        }

        /// <summary>
        /// HttpUploadFile
        /// </summary>
        /// <param name="url"></param>
        /// <param name="files"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string UploadFiles(string url, string[] files, NameValueCollection data, Encoding encoding)
        {
            var fileKeys = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                fileKeys.Add("file" + i);
            }

            return UploadFiles(url, data, files, fileKeys, "application/octet-stream", encoding);
        }
        #endregion

        #region UploadImage

        public static string UploadImage(string url, string file, NameValueCollection data)
        {
            var files = new[] { file };
            var fileKeys = new List<string>();
            for (int i = 0; i < files.Length; i++)
            {
                fileKeys.Add("uploadimg");
            }
            return UploadImages(url, files, fileKeys, data, DefaultEncode);
        }
        public static string UploadImages(string url, string[] files, List<string> fileKeys, NameValueCollection data,
            Encoding encoding)
        {
            return UploadFiles(url, data, files, fileKeys, "image/jpeg", encoding);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">访问地址</param>
        /// <param name="fieldValues">数据字典</param>
        /// <param name="files">文件列表</param>
        /// <param name="fileKeys">"uploadimg"、"file0"</param>
        /// <param name="fileContentType">"image/jpeg"</param>
        /// <param name="encoding">编码，如UTF8</param>
        /// <returns></returns>
        public static string UploadFiles(string url, NameValueCollection fieldValues, string[] files, List<string> fileKeys, string fileContentType, Encoding encoding)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endbytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            //1.HttpWebRequest
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            request.ContentType = $"multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            using (Stream stream = request.GetRequestStream())
            {
                //1.1 key/value
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                if (fieldValues != null)
                {
                    foreach (string key in fieldValues.Keys)
                    {
                        stream.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, fieldValues[key]);
                        byte[] formitembytes = encoding.GetBytes(formitem);
                        stream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                //1.2 file
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    //
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string header = string.Format(headerTemplate, fileKeys[i], Path.GetFileName(files[i]), fileContentType);
                    byte[] headerbytes = encoding.GetBytes(header);
                    stream.Write(headerbytes, 0, headerbytes.Length);
                    using (FileStream fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                //1.3 form end
                stream.Write(endbytes, 0, endbytes.Length);
            }
            //2.WebResponse
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader stream = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                var result = stream.ReadToEnd();
                var decodeResult = UnicodeHelper.Unicode2String(result);
                return decodeResult;
            }
        }
    }
}
