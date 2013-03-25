using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Reflection;
using System.Net;
using System.IO;
using Hadouken.IO;
using Hadouken.Configuration;
using Ionic.Zip;
using NLog;
using Hadouken.Security;

namespace Hadouken.Http.HttpServer
{
    public class DefaultHttpServer : IHttpServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFileSystem _fileSystem;
        private readonly IKeyValueStore _keyValueStore;

        private HttpListener _listener;
        private string _webUIPath;

        private static readonly string TokenCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly int TokenLength = 40;
        
        public DefaultHttpServer(IKeyValueStore keyValueStore, IFileSystem fileSystem)
        {
            _keyValueStore = keyValueStore;
            _fileSystem = fileSystem;
        }

        public void Start()
        {
            var binding = GetBinding();

            _listener = new HttpListener();
            _listener.Prefixes.Add(binding);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            UnzipWebUI();

            try
            {
                _listener.Start();
            } catch(HttpListenerException e)
            {
                Logger.FatalException("Could not start the HTTP server interface. HTTP server NOT up and running.", e);
                return;
            }

            ReceiveLoop();

            Logger.Info("HTTP server up and running on address " + ListenUri);
        }

        public void Stop()
        {
            if (_listener == null) return;

            _listener.Stop();
            _listener.Close();
            _listener = null;
        }

        public Uri ListenUri
        {
            get
            {
                if (_listener.IsListening)
                {
                    return new Uri(_listener.Prefixes.First());
                }

                return null;
            }
        }

        private string GetBinding()
        {
            var binding = "http://+:{port}/";
            var port = -1;

            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Url"))
            {
                binding = HdknConfig.ConfigManager["WebUI.Url"];
            }

            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Port"))
            {
                port = Convert.ToInt32(HdknConfig.ConfigManager["WebUI.Port"]);
            }
            else
            {
                port = _keyValueStore.Get("webui.port", -1, StorageLocation.Registry);
                
            }

            if (port == -1)
                throw new InvalidDataException("Could not find a port for the web ui.");

            return binding.Replace("{port}", port.ToString());;
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
                Logger.Trace("Incoming request to {0}", context.Request.Url);

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

            Logger.Debug("Checking if webui.zip exists at {0}", uiZip);

            if (_fileSystem.FileExists(uiZip))
            {
                string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                _fileSystem.CreateDirectory(path);

                Logger.Info("Extracting webui.zip to {0}", path);

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

            if (_fileSystem.FileExists(path))
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

                return new ContentResult { Content = _fileSystem.ReadAllBytes(path), ContentType = contentType };
            }

            return null;
        }

        private bool IsAuthenticatedUser(IHttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var id = (HttpListenerBasicIdentity)context.User.Identity;

                var usr = _keyValueStore.Get<string>("auth.username");
                var pwd = _keyValueStore.Get<string>("auth.password");

                return (id.Name == usr && Hash.Generate(id.Password) == pwd);
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
                    Logger.ErrorException(String.Format("Could not execute action {0}", action.GetType().FullName), e);
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
