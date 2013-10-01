using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Hadouken.Framework.Http
{
    public class HttpFileServer : IHttpFileServer
    {
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly string _baseDirectory;
        private readonly string _uriPrefix;

        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>()
        {
            {".html", "text/html"},
            {".css", "text/css"},
            {".js", "text/javascript"},
            {".woff", "application/octet-stream"},
            {".ttf", "application/octet-stream"},
            {".svg", "application/octet-stream"}
        };

        public HttpFileServer(string listenUri, string baseDirectory)
            : this(listenUri, baseDirectory, String.Empty)
        {
        }

        public HttpFileServer(string listenUri, string baseDirectory, string uriPrefix)
        {
            _baseDirectory = baseDirectory;
            _uriPrefix = uriPrefix;
            _httpListener.Prefixes.Add(listenUri);
        }

        public void Open()
        {
            _httpListener.Start();
            _httpListener.BeginGetContext(GetContext, null);
        }

        public void Close()
        {
            _httpListener.Close();
        }

        private void GetContext(IAsyncResult ar)
        {
            var context = _httpListener.EndGetContext(ar);
            Task.Run(() => ProcessContext(context));
            _httpListener.BeginGetContext(GetContext, null);
        }

        private void ProcessContext(HttpListenerContext context)
        {
            var path = _baseDirectory +
                       context.Request.Url.AbsolutePath.Substring(_uriPrefix.EndsWith("/")
                           ? _uriPrefix.Length - 1
                           : _uriPrefix.Length);

            var extension = Path.GetExtension(path);

            if (File.Exists(path))
            {
                using (var reader = new StreamReader(path))
                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.Write(reader.ReadToEnd());
                }

                context.Response.ContentType = MimeTypes.ContainsKey(extension) ? MimeTypes[extension] : "text/plain";
                context.Response.StatusCode = 200;
            }
            else
            {
                context.Response.StatusCode = 404;
            }

            context.Response.OutputStream.Close();
            context.Response.Close();
        }
    }
}