using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private IEnumerable<IApiAction> _actions; 

        private HttpListener _listener;
        private string _webUIPath;

        private static readonly string TokenCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly int TokenLength = 40;
        
        public DefaultHttpServer(IKeyValueStore kvs, IFileSystem fs, IEnumerable<IApiAction> actions)
        {
            _fs = fs;
            _kvs = kvs;
            _actions = actions;

            var binding = HdknConfig.ConfigManager["WebUI.Url"];

            _listener = new HttpListener();
            _listener.Prefixes.Add(binding);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            UnzipWebUI();
        }

        public void Start()
        {
            _listener.Start();
            ReceiveLoop();

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

        private void ReceiveLoop()
        {
            _listener.BeginGetContext(ar =>
            {
                HttpListenerContext context;

                try
                {
                    context = _listener.EndGetContext(ar);
                }
                catch(Exception)
                {
                    return;
                }

                ReceiveLoop();

                new Task(() => ProcessRequest(new HttpContext(context))).Start();

            }, null);
        }

        private void ProcessRequest(IHttpContext context)
        {
            try
            {
                _logger.Trace("Incoming request to {0}", context.Request.Url);

                if(IsAuthenticatedUser(context))
                {
                    string url = context.Request.Url.AbsolutePath;

                    var result = (((url == "/api" || url == "/api/") && context.Request.QueryString["action"] != null)
                                      ? FindAndExecuteAction(context)
                                      : CheckFileSystem(context));

                    if (result != null)
                    {
                        result.Execute(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.StatusDescription = "404 - File not found";
                    }
                }
                else
                {
                    context.Response.Unauthorized();
                }

                context.Response.Close();
            }
            catch(Exception e)
            {
                context.Response.Error(e);
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
            else
            {
                _webUIPath = Path.Combine(_webUIPath, "WebUI");
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

        private bool IsAuthenticatedUser(IHttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var id = (HttpListenerBasicIdentity)context.User.Identity;

                string usr = HdknConfig.ConfigManager["Auth.Username"];
                string pwd = HdknConfig.ConfigManager["Auth.Password"];

                return (id.Name == usr && id.Password == pwd);
            }

            return false;
        }

        private ActionResult FindAndExecuteAction(IHttpContext context)
        {
            string actionName = context.Request.QueryString["action"];

            if (actionName == "gettoken")
                return GenerateCSRFToken();

            var action = (from a in Kernel.Resolver.GetAll<IApiAction>()
                          where a.GetType().HasAttribute<ApiActionAttribute>()
                          let attr = a.GetType().GetAttribute<ApiActionAttribute>()
                          where attr != null && attr.Name == actionName
                          select a).SingleOrDefault();

            if (action != null)
            {
                try
                {
                    action.Context = context;
                    return action.Execute();
                }
                catch (Exception e)
                {
                    _logger.ErrorException(String.Format("Could not execute action {0}", action.GetType().FullName), e);
                }
            }

            return null;
        }

        private ActionResult GenerateCSRFToken()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var sb = new StringBuilder();

            for (int i = 0; i < TokenLength; i++)
            {
                sb.Append(TokenCharacters[rnd.Next(0, TokenCharacters.Length - 1)]);
            }

            return new JsonResult() { Data = sb.ToString() };
        }
    }
}
