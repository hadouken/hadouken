using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Hadouken.Common.Http
{
    public interface IHttpRequest
    {
        string[] AcceptTypes { get; }
        int ClientCertificateError { get; }
        Encoding ContentEncoding { get; }
        long ContentLength64 { get; }
        string ContentType { get; }
        CookieCollection Cookies { get; }

        NameValueCollection Form { get; }

        bool HasEntityBody { get; }
        NameValueCollection Headers { get; }
        string HttpMethod { get; }
        Stream InputStream { get; }
        bool IsAuthenticated { get; }
        bool IsLocal { get; }
        bool IsSecureConnection { get; }
        bool KeepAlive { get; }
        IPEndPoint LocalEndPoint { get; }
        Version ProtocolVersion { get; }
        NameValueCollection QueryString { get; }
        string RawUrl { get; }
        IPEndPoint RemoteEndPoint { get; }
        Guid RequestTraceIdentifier { get; }
        string ServiceName { get; }
        TransportContext TransportContext { get; }
        Uri Url { get; }
        Uri UrlReferrer { get; }
        string UserAgent { get; }
        string UserHostAddress { get; }
        string UserHostName { get; }
        string[] UserLanguages { get; }
    }
}
