using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hadouken.Framework.Http
{
    public class HttpFileServer : IHttpFileServer
    {
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly string _baseDirectory;
        private readonly string _uriPrefix;

        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private readonly Task _workerTask;

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

            _workerTask = new Task(ct => Run(_cancellationToken.Token), _cancellationToken.Token);
        }

        public void Open()
        {
            _workerTask.Start();
        }

        public void Close()
        {
            _cancellationToken.Cancel();
            _workerTask.Wait();
        }

        private async void Run(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _httpListener.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                var context = await _httpListener.GetContextAsync();

                if (cancellationToken.IsCancellationRequested)
                    break;

                ProcessContext(context);
            }

            _httpListener.Close();
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