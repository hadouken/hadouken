using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Web.CoffeeScript;

namespace Hadouken.Plugins.Web.Http
{
    public class HttpFileServer : IHttpFileServer
    {
        private static readonly Regex PluginRegex = new Regex("^/plugins/(?<pluginId>[a-zA-Z0-9\\.]*?)/(?<path>.*)$");

        private readonly IJsonRpcClient _rpcClient;
        private readonly ICoffeeCompiler _coffeeCompiler;
        private readonly HttpListener _httpListener;

        public HttpFileServer(string listenUri, IJsonRpcClient rpcClient, ICoffeeCompiler coffeeCompiler)
        {
            _rpcClient = rpcClient;
            _coffeeCompiler = coffeeCompiler;

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(listenUri);
        }

        public void Start()
        {
            _httpListener.Start();
            _httpListener.BeginGetContext(GetContext, null);
        }

        private void GetContext(IAsyncResult ar)
        {
            try
            {
                var context = _httpListener.EndGetContext(ar);

                _httpListener.BeginGetContext(GetContext, null);
                Task.Run(() => ProcessContext(context));
            }
            catch (ObjectDisposedException disposedException)
            {
                return;
            }
            catch (Exception exception)
            {
                // TODO: Add logging. Should we still call BeginGetContext here?
            }
        }

        private void ProcessContext(HttpListenerContext context)
        {
            var path = "UI" + context.Request.Url.AbsolutePath;
            var pluginId = "core.web";

            if (PluginRegex.IsMatch(context.Request.Url.AbsolutePath))
            {
                var match = PluginRegex.Match(context.Request.Url.AbsolutePath);

                pluginId = match.Groups["pluginId"].Value;
                path = "UI/" + match.Groups["path"].Value;
            }

            var fileContents = _rpcClient.CallAsync<byte[]>(
                "plugins.getFileContents",
                new[] { pluginId, path })
                .Result;

            if (fileContents == null)
            {
                context.Response.StatusCode = 404;
                context.Response.OutputStream.Close();
                context.Response.Close();
            }
            else
            {
                var mapping = MimeMapping.GetMimeMapping(path);

                if (path.EndsWith(".coffee"))
                {
                    var coffeeScript = _coffeeCompiler.Compile(Encoding.UTF8.GetString(fileContents));
                    fileContents = Encoding.UTF8.GetBytes(coffeeScript);

                    mapping = "text/javascript";
                }

                context.Response.ContentType = mapping;
                context.Response.StatusCode = 200;
                context.Response.OutputStream.Write(fileContents, 0, fileContents.Length);
                context.Response.OutputStream.Close();
                context.Response.Close();
            }
        }

        public void Stop()
        {
            _httpListener.Stop();
        }
    }
}