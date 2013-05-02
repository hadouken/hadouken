using Hadouken.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Hadouken.Common.Http.HttpListener
{
    public class HttpListenerServer : IHttpFileSystemServer
    {
        private readonly System.Net.HttpListener _httpListener;
        private readonly HttpListenerBasicIdentity _credential;
        private readonly string _basePath;

        private readonly IFileSystem _fileSystem;

        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>()
            {
                {".css", "text/css"},
                {".js",  "text/javascript"},
                {".html", "text/html"}
            };

        public HttpListenerServer(IFileSystem fileSystem, string binding, NetworkCredential credential, string basePath)
        {
            _fileSystem = fileSystem;
            _basePath = basePath;

            _httpListener = new System.Net.HttpListener();
            _httpListener.Prefixes.Add(binding);
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            _credential = new HttpListenerBasicIdentity(credential.UserName, credential.Password);            
        }

        public void Start()
        {
            if (_httpListener == null) return;

            _httpListener.Start();
            _httpListener.BeginGetContext(BeginGetContext, null);
        }

        private void BeginGetContext(IAsyncResult ar)
        {
            try
            {
                var context = _httpListener.EndGetContext(ar);

                if (!IsAuthenticated(context.User.Identity as HttpListenerBasicIdentity))
                    return;

                Task.Factory.StartNew(() => OnHttpRequest(context));

                _httpListener.BeginGetContext(BeginGetContext, null);
            }
            catch (HttpListenerException)
            {
                //TODO: better catch clause
            }
        }

        private void OnHttpRequest(HttpListenerContext context)
        {
            var pathSegments = context.Request.Url.Segments.Skip(1).Select(s => s.Replace("/", "")).ToList();
            pathSegments.Insert(0, _basePath);

            // Check file system for file
            var file = Path.Combine(pathSegments.ToArray());

            if (_fileSystem.FileExists(file))
            {
                var contentType = "";

                if (MimeTypes.ContainsKey(Path.GetExtension(file)))
                    contentType = MimeTypes[Path.GetExtension(file)];

                var data = _fileSystem.ReadAllBytes(file);

                context.Response.StatusCode = 200;
                context.Response.ContentType = contentType;
                context.Response.OutputStream.Write(data, 0, data.Length);
            }
            else
            {
                context.Response.StatusCode = 404;
            }

            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        private bool IsAuthenticated(HttpListenerBasicIdentity identity)
        {
            if (identity == null)
                return false;

            return (identity.Name == _credential.Name && identity.Password == _credential.Password);
        }

        public void Stop()
        {
            _httpListener.Stop();
        }
    }
}
