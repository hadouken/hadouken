using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Hadouken.Framework.Events;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.Web.Http
{
    public class HttpFileServer : IHttpFileServer
    {
        private static readonly Regex PluginRegex = new Regex("^/plugins/(?<pluginId>[a-zA-Z0-9\\.]*?)/(?<path>.*)$");

        private readonly IJsonRpcClient _rpcClient;
        private readonly IEventListener _eventListener;
        private readonly HttpListener _httpListener;
        private NetworkCredential _credentials;

        public HttpFileServer(string listenUri, IJsonRpcClient rpcClient, IEventListener eventListener)
        {
            _rpcClient = rpcClient;
            _eventListener = eventListener;

            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add(listenUri);
        }

        public void Start()
        {
            _eventListener.Subscribe<AuthChangedEventArgs>("auth.changed",
                args => SetCredentials(args.UserName, args.HashedPassword));

            _httpListener.Start();
            _httpListener.BeginGetContext(GetContext, null);
        }

        public void SetCredentials(string userName, string hashedPassword)
        {
            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(hashedPassword)) return;

            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            _credentials = new NetworkCredential(userName, hashedPassword);
        }

        private bool IsAuthenticatedUser(HttpListenerContext context)
        {
            if (_httpListener.AuthenticationSchemes == AuthenticationSchemes.None)
                return true;

            if (_credentials == null)
                return true;

            var id = (HttpListenerBasicIdentity)context.User.Identity;
            var passwordHash = ComputeHash(id.Password);

            return id.Name == _credentials.UserName && passwordHash == _credentials.Password;
        }

        private string ComputeHash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var sha256 = new SHA256Managed();
            var hash = sha256.ComputeHash(bytes);
            var sb = new StringBuilder();

            foreach (var b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
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
            }
            catch (Exception exception)
            {
                // TODO: Add logging. Should we still call BeginGetContext here?
            }
        }

        private void ProcessContext(HttpListenerContext context)
        {
            if (!IsAuthenticatedUser(context))
            {
                context.Response.StatusCode = 401;
                context.Response.OutputStream.Close();
                context.Response.Close();

                return;
            }

            var file = context.Request.Url.AbsolutePath;

            if (file == "/")
                file = "/index.html";

            var path = "UI" + file;
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