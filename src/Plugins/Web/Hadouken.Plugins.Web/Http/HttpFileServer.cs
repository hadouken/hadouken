using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Hadouken.Framework.Events;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Security;
using NLog;

namespace Hadouken.Plugins.Web.Http
{
    public class HttpFileServer : IHttpFileServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex PluginRegex = new Regex("^/plugins/(?<pluginId>[a-zA-Z0-9\\.]*?)/(?<path>.*)$");
        private static readonly string DefaultPlugin = "core.web";
        private static readonly string DefaultFile = "/index.html";
        private static readonly string DefaultPath = "UI";
        private static readonly string RootPath = "/";

        private readonly IHashProvider _hashProvider;
        private readonly IJsonRpcClient _rpcClient;
        private readonly IEventListener _eventListener;
        private readonly HttpListener _httpListener;
        private NetworkCredential _credentials;

        public HttpFileServer(string listenUri, IJsonRpcClient rpcClient, IEventListener eventListener)
        {
            _hashProvider = HashProvider.GetDefault();
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

            Logger.Info("Changing credentials");

            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            _credentials = new NetworkCredential(userName, hashedPassword);
        }

        private bool IsAuthenticatedUser(HttpListenerContext context)
        {
            if (_httpListener.AuthenticationSchemes != AuthenticationSchemes.Basic)
                return true;

            if (_credentials == null)
                return true;

            var id = (HttpListenerBasicIdentity)context.User.Identity;
            var passwordHash = _hashProvider.ComputeHash(id.Password);

            return id.Name == _credentials.UserName && passwordHash == _credentials.Password;
        }

        private void GetContext(IAsyncResult ar)
        {
            try
            {
                var context = _httpListener.EndGetContext(ar);

                _httpListener.BeginGetContext(GetContext, null);
                Task.Run(() => ProcessContext(context));
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exception)
            {
                Logger.FatalException("Unhandled exception occured.", exception);
            }
        }

        private void ProcessContext(HttpListenerContext context)
        {
            if (!IsAuthenticatedUser(context))
            {
                Logger.Info("Unauthorized access attempt from " + context.Request.RemoteEndPoint);

                context.Response.StatusCode = 401;
                context.Response.OutputStream.Close();
                context.Response.Close();

                return;
            }

            var file = context.Request.Url.AbsolutePath;

            if (file == RootPath)
                file = DefaultFile;

            var path = string.Concat(DefaultPath, file);
            var pluginId = DefaultPlugin;

            if (PluginRegex.IsMatch(context.Request.Url.AbsolutePath))
            {
                var match = PluginRegex.Match(context.Request.Url.AbsolutePath);

                pluginId = match.Groups["pluginId"].Value;
                path = string.Concat("UI", "/", match.Groups["path"].Value);
            }

            var fileContents = _rpcClient.Call<byte[]>(
                "plugins.getFileContents",
                new[] {pluginId, path});

            if (fileContents == null)
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                var mapping = MimeMapping.GetMimeMapping(path);

                context.Response.ContentType = mapping;
                context.Response.StatusCode = 200;
                context.Response.OutputStream.Write(fileContents, 0, fileContents.Length);
            }

            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        public void Stop()
        {
            _httpListener.Stop();
        }
    }
}