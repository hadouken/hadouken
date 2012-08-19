using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Http;
using Hadouken.Reflection;

using System.Net;
using System.IO;
using Hadouken.IO;
using Hadouken.Data;
using System.Configuration;
using Hadouken.Configuration;
using System.Text.RegularExpressions;
using System.Reflection;

using NLog;
using Ionic.Zip;

namespace Hadouken.Impl.Http
{
    public class DefaultHttpServer : IHttpServer
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private List<ActionCacheItem> _cache = new List<ActionCacheItem>();

        private IFileSystem _fs;
        private IKeyValueStore _kvs;

        private HttpListener _listener;
        private string _webUIPath;
        
        public DefaultHttpServer(IKeyValueStore kvs, IFileSystem fs)
        {
            _fs = fs;
            _kvs = kvs;
            _listener = new HttpListener();
        }

        public void Start()
        {
            UnzipWebUI();

            var binding = ConfigurationManager.AppSettings["WebUI.Url"];

            _listener.Prefixes.Add(binding);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            _listener.Start();

            _listener.BeginGetContext(GetContext, null);

            _logger.Info("HTTP server up and running");
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public Uri ListenUri
        {
            get
            {
                if (_listener.IsListening)
                {
                    return new Uri(_listener.Prefixes.FirstOrDefault());
                }

                return null;
            }
        }

        private void UnzipWebUI()
        {
            _webUIPath = HdknConfig.GetPath("Paths.WebUI");

            string uiZip = Path.Combine(_webUIPath, "webui.zip");

            _logger.Debug("Checking if webui.zip exists at {0}", uiZip);

            if (_fs.FileExists(uiZip))
            {
                string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                _fs.CreateDirectory(path);

                _logger.Info("Extracting webui.zip to {0}", path);

                using (var zip = ZipFile.Read(uiZip))
                {
                    zip.ExtractAll(path);
                }

                _webUIPath = path;
            }
        }

        private void GetContext(IAsyncResult ar)
        {
            try
            {
                var context = _listener.EndGetContext(ar);

                if (context != null)
                {
                    // authenticate user
                    if (IsAuthenticatedUser(context))
                    {
                        var httpContext = new HttpContext(context);
                        string url = httpContext.Request.Url.AbsolutePath;

                        var result = (((url == "/api" || url == "/api/") && httpContext.Request.QueryString["action"] != null) ? FindAndExecuteAction(httpContext) : CheckFileSystem(httpContext));

                        if (result != null)
                        {
                            result.Execute(httpContext);
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            context.Response.StatusDescription = "404 - File not found";
                        }
                    }
                    else
                    {
                        context.Response.ContentType = "text/html";
                        context.Response.StatusCode = 401;

                        byte[] unauthorized = Encoding.UTF8.GetBytes("<h1>401 - Unauthorized</h1>");

                        context.Response.OutputStream.Write(unauthorized, 0, unauthorized.Length);
                    }

                    context.Response.OutputStream.Close();

                }

                _listener.BeginGetContext(GetContext, null);
            }
            catch (ObjectDisposedException)
            {
                // probably closing
                return;
            }
        }

        private ActionResult CheckFileSystem(IHttpContext context)
        {
            string path = _webUIPath + (context.Request.Url.AbsolutePath == "/" ? "/index.html" : context.Request.Url.AbsolutePath);

            if (_fs.FileExists(path))
            {
                string contentType = "text/html";

                switch (Path.GetExtension(path))
                {
                    case ".css":
                        contentType = "text/css";
                        break;

                    case ".js":
                        contentType = "text/javascript";
                        break;

                    case ".png":
                        contentType = "image/png";
                        break;

                    case ".gif":
                        contentType = "image/gif";
                        break;
                }

                return new ContentResult { Content = _fs.ReadAllBytes(path), ContentType = contentType };
            }

            return null;
        }

        private bool IsAuthenticatedUser(HttpListenerContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var id = (HttpListenerBasicIdentity)context.User.Identity;

                string usr = _kvs.Get<string>("http.auth.username", "hdkn");
                string pwd = _kvs.Get<string>("http.auth.password", "hdkn");

                return (id.Name == usr && id.Password == pwd);
            }

            return false;
        }

        private ActionResult FindAndExecuteAction(IHttpContext context)
        {
            string actionName = context.Request.QueryString["action"];
            var actions = Kernel.GetAll<IApiAction>();

            var action = (from a in actions
                          let attr = a.GetType().GetAttribute<ApiActionAttribute>()
                          where attr != null && attr.Name == actionName
                          select a).SingleOrDefault();

            if (action != null)
            {
                return action.Execute(context);
            }

            return null;
        }
    }
}
