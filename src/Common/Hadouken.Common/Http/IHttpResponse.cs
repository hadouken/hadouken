using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Hadouken.Common.Http
{
    public interface IHttpResponse
    {
        Encoding ContentEncoding { get; set; }
        long ContentLength64 { get; set; }
        string ContentType { get; set; }
        CookieCollection Cookies { get; set; }
        WebHeaderCollection Headers { get; set; }
        bool KeepAlive { get; set; }
        Stream OutputStream { get; }
        Version ProtocolVersion { get; set; }
        string RedirectLocation { get; set; }
        bool SendChunked { get; set; }
        int StatusCode { get; set; }
        string StatusDescription { get; set; }

        void Abort();
        void AddHeader(string name, string value);
        void AppendCookie(Cookie cookie);
        void AppendHeader(string name, string value);
        void Close();
        void Close(byte[] responseEntity, bool willBlock);
        void Redirect(string url);
        void SetCookie(Cookie cookie);
    }
}
