using System;
using System.Text;
using System.Net;

namespace Hadouken.Common.Http.HttpListener
{
    public class HttpResponse : IHttpResponse
    {
        private HttpListenerResponse _response;

        public HttpResponse(HttpListenerResponse listenerResponse)
        {
            _response = listenerResponse;
        }

        public Encoding ContentEncoding
        {
            get { return _response.ContentEncoding; }
            set { _response.ContentEncoding = value; }
        }

        public long ContentLength64
        {
            get { return _response.ContentLength64; }
            set { _response.ContentLength64 = value; }
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public CookieCollection Cookies
        {
            get { return _response.Cookies; }
            set { _response.Cookies = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _response.Headers; }
            set { _response.Headers = value; }
        }

        public bool KeepAlive
        {
            get { return _response.KeepAlive; }
            set { _response.KeepAlive = value; }
        }

        public System.IO.Stream OutputStream
        {
            get { return _response.OutputStream; }
        }

        public Version ProtocolVersion
        {
            get { return _response.ProtocolVersion; }
            set { _response.ProtocolVersion = value; }
        }

        public string RedirectLocation
        {
            get { return _response.RedirectLocation; }
            set { _response.RedirectLocation = value; }
        }

        public bool SendChunked
        {
            get { return _response.SendChunked; }
            set { _response.SendChunked = value; }
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public string StatusDescription
        {
            get { return _response.StatusDescription; }
            set { _response.StatusDescription = value; }
        }

        public void Abort()
        {
            _response.Abort();
        }

        public void AddHeader(string name, string value)
        {
            _response.AddHeader(name, value);
        }

        public void AppendCookie(Cookie cookie)
        {
            _response.AppendCookie(cookie);
        }

        public void AppendHeader(string name, string value)
        {
            _response.AppendHeader(name, value);
        }

        public void Close()
        {
            _response.Close();
        }

        public void Close(byte[] responseEntity, bool willBlock)
        {
            _response.Close(responseEntity, willBlock);
        }

        public void Redirect(string url)
        {
            _response.Redirect(url);
        }

        public void SetCookie(Cookie cookie)
        {
            _response.SetCookie(cookie);
        }
    }
}
