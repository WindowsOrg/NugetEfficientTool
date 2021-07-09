using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    [DataContract]
    public class HttpRequest
    {
        protected static int DefaultTimeOut = 30000;
        private int _timeOut;

        public string Url { get; set; } = string.Empty;

        internal string Host { get; set; } = string.Empty;

        public string Referer { get; set; }

        public CookieCollection CookieCollection { get; set; } = new CookieCollection();

        public WebHeaderCollection HeaderCollection { get; set; } = new WebHeaderCollection();

        /// <summary>超时时间</summary>
        public int TimeOut
        {
            get
            {
                if (this._timeOut <= 0)
                    this._timeOut = HttpRequest.DefaultTimeOut;
                return this._timeOut;
            }
            set
            {
                this._timeOut = value;
            }
        }

        /// <summary>获取请求的ContentType</summary>
        public virtual string ContentType
        {
            get
            {
                return "";
            }
        }

        public string UserAgent { get; set; }

        public string Method { get; internal set; }

        /// <summary>获取请求携带的数据</summary>
        /// <returns></returns>
        public virtual byte[] GetRequestData()
        {
            return new byte[0];
        }

    }
}
