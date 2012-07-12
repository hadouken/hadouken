using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Http;
using Hadouken.Logging;
using Hadouken.Reflection;

using System.Net;
using System.IO;
using Hadouken.IO;
using Hadouken.Data;
using System.Configuration;
using Hadouken.Configuration;

namespace Hadouken.Impl.Http
{
    public class DefaultHttpServer : IHttpServer
    {
        private Dictionary<string, ActionCacheItem> _cache = new Dictionary<string, ActionCacheItem>();

        private ILogger _log;
        private IFileSystem _fs;
        private IDataRepository _data;

        private HttpListener _listener;
        
        public DefaultHttpServer(IDataRepository data, IFileSystem fs, ILogger logger)
        {
            _fs = fs;
            _log = logger;
            _data = data;
            _listener = new HttpListener();
        }

        public void Start()
        {
            var binding = _data.GetSetting("http.binding", "http://localhost:12012/");

            _listener.Prefixes.Add(binding);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            _listener.Start();

            _listener.BeginGetContext(GetContext, null);

            _log.Info("HTTP server up and running");
        }

        public void Stop()
        {
            _listener.Stop();
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

                        ActionResult result = FindAndExecuteController(httpContext);

                        if (result != null)
                        {
                            result.Execute(httpContext);
                        }
                        else
                        {
                            // check file system
                            result = CheckFileSystem(httpContext);

                            if (result != null)
                            {
                                result.Execute(httpContext);
                            }
                            else
                            {
                                context.Response.StatusCode = 404;

                                byte[] notFound = Encoding.UTF8.GetBytes("404 - Not found!");

                                context.Response.OutputStream.Write(notFound, 0, notFound.Length);
                            }
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
            catch (HttpListenerException e)
            {
                // probably closing
                return;
            }
        }

        private ActionResult CheckFileSystem(IHttpContext context)
        {
            string webUIPath = HdknConfig.GetPath("Paths.WebUI");

            string path = Path.Combine(webUIPath, "Content") + context.Request.Url.AbsolutePath;

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

                string usr = _data.GetSetting("http.auth.username", "hdkn");
                string pwd = _data.GetSetting("http.auth.password", "hdkn");

                return (id.Name == usr && id.Password == pwd);
            }

            return false;
        }

        private ActionResult FindAndExecuteController(IHttpContext context)
        {
            if (_cache.ContainsKey(context.Request.Url.AbsolutePath))
            {
                var cacheItem = _cache[context.Request.Url.AbsolutePath];
                IController instance = (IController)Kernel.Get(cacheItem.Controller);
                ((Controller)instance).Context = context;

                return cacheItem.Action.Invoke(instance, null) as ActionResult;
            }
            else
            {
                var handlers = Kernel.GetAll<IController>();

                foreach (var instance in handlers)
                {
                    ((Controller)instance).Context = context;

                    var method = (from mi in instance.GetType().GetMethods()
                                  where mi.HasAttribute<RouteAttribute>()
                                  where mi.HasAttribute<HttpMethodAttribute>()
                                  let methodAttribute = mi.GetAttribute<HttpMethodAttribute>()
                                  let routeAttribute = mi.GetAttribute<RouteAttribute>()
                                  where routeAttribute.Route == context.Request.Url.AbsolutePath && methodAttribute.Method == context.Request.HttpMethod
                                  select mi).FirstOrDefault();

                    if (method != null)
                    {
                        _cache.Add(context.Request.Url.AbsolutePath, new ActionCacheItem() { Action = method, Controller = instance.GetType() });
                        return method.Invoke(instance, null) as ActionResult;
                    }
                }
            }

            return null;
        }
    }
}
