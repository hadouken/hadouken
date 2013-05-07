using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;

namespace Hadouken.Http.HttpServer
{
    public class HttpRequest : IHttpRequest
    {
        private HttpListenerRequest _request;
        private FormData _formData;
        private List<IHttpPostedFile> _files = new List<IHttpPostedFile>();

        public HttpRequest(HttpListenerRequest listenerRequest)
        {
            _request = listenerRequest;
            _formData = new FormData(listenerRequest);

            LoadFormData();
        }

        private void LoadFormData()
        {
            Form = new NameValueCollection(_formData.FormValues);

            foreach (var file in _formData.Files)
            {
                _files.Add(new HttpPostedFile(file.FileData.Length, file.ContentType, file.FileName, new MemoryStream(file.FileData, false)));
            }
        }

        public string[] AcceptTypes
        {
            get { return _request.AcceptTypes; }
        }

        public int ClientCertificateError
        {
            get { return _request.ClientCertificateError; }
        }

        public Encoding ContentEncoding
        {
            get { return _request.ContentEncoding; }
        }

        public long ContentLength64
        {
            get { return _request.ContentLength64; }
        }

        public string ContentType
        {
            get { return _request.ContentType; }
        }

        public System.Net.CookieCollection Cookies
        {
            get { return _request.Cookies; }
        }

        public NameValueCollection Form { get; private set; }

        public IEnumerable<IHttpPostedFile> Files
        {
            get { return _files; }
        }

        public bool HasEntityBody
        {
            get { return _request.HasEntityBody; }
        }

        public System.Collections.Specialized.NameValueCollection Headers
        {
            get { return _request.Headers; }
        }

        public string HttpMethod
        {
            get { return _request.HttpMethod; }
        }

        public System.IO.Stream InputStream
        {
            get { return _request.InputStream; }
        }

        public bool IsAuthenticated
        {
            get { return _request.IsAuthenticated; }
        }

        public bool IsLocal
        {
            get { return _request.IsLocal; }
        }

        public bool IsSecureConnection
        {
            get { return _request.IsSecureConnection; }
        }

        public bool KeepAlive
        {
            get { return _request.KeepAlive; }
        }

        public System.Net.IPEndPoint LocalEndPoint
        {
            get { return _request.LocalEndPoint; }
        }

        public Version ProtocolVersion
        {
            get { return _request.ProtocolVersion; }
        }

        public System.Collections.Specialized.NameValueCollection QueryString
        {
            get { return _request.QueryString; }
        }

        public string RawUrl
        {
            get { return _request.RawUrl; }
        }

        public System.Net.IPEndPoint RemoteEndPoint
        {
            get { return _request.RemoteEndPoint; }
        }

        public Guid RequestTraceIdentifier
        {
            get { return _request.RequestTraceIdentifier; }
        }

        public string ServiceName
        {
            get { return _request.ServiceName; }
        }

        public System.Net.TransportContext TransportContext
        {
            get { return _request.TransportContext; }
        }

        public Uri Url
        {
            get { return _request.Url; }
        }

        public Uri UrlReferrer
        {
            get { return _request.UrlReferrer; }
        }

        public string UserAgent
        {
            get { return _request.UserAgent; }
        }

        public string UserHostAddress
        {
            get { return _request.UserHostAddress; }
        }

        public string UserHostName
        {
            get { return _request.UserHostName; }
        }

        public string[] UserLanguages
        {
            get { return _request.UserLanguages; }
        }
    }
}
