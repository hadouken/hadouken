using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Hadouken.Framework.Events;
using Hadouken.Framework.Http.TypeScript;
using NLog;

namespace Hadouken.Framework.Http
{
    public class AuthChangedEventArgs
    {
        public string UserName { get; set; }

        public string HashedPassword { get; set; }
    }

    public class HttpFileServer : IHttpFileServer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly HttpListener _httpListener = new HttpListener();
        private readonly ITypeScriptCompiler _typeScriptCompiler = TypeScriptCompiler.Create();
        private readonly IEventListener _eventListener;
        private readonly string _baseDirectory;
        private readonly string _uriPrefix;

        private NetworkCredential _credentials;

        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private readonly Task _workerTask;

        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>()
        {
            {".html", "text/html"},
            {".css", "text/css"},
            {".js", "text/javascript"},
            {".woff", "application/font-woff"},
            {".ttf", "application/x-font-ttf"},
            {".svg", "application/octet-stream"}
        };

        public HttpFileServer(string listenUri, string baseDirectory, IEventListener eventListener)
        {
            if (!listenUri.EndsWith("/"))
                listenUri = listenUri + "/";

            var virtualPath = Regex.Replace(listenUri, "^http://.*:[0-9]+(.*)$", "$1");

            _baseDirectory = baseDirectory;
            _eventListener = eventListener;
            _uriPrefix = virtualPath;
            _httpListener.Prefixes.Add(listenUri);

            _workerTask = new Task(ct => Run(_cancellationToken.Token), _cancellationToken.Token);
        }

        public void SetCredentials(string username, string password)
        {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) return;

            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            _credentials = new NetworkCredential(username, password);
        }

        public void Open()
        {
            _eventListener.Subscribe<AuthChangedEventArgs>("auth.changed",
                args => SetCredentials(args.UserName, args.HashedPassword));

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

                try
                {
                    ProcessContext(context);
                }
                catch (Exception e)
                {
                    Logger.ErrorException("Error when processing request.", e);
                }
            }

            _httpListener.Close();
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

            var requestedPath = context.Request.Url.AbsolutePath == "/"
                ? "/index.html"
                : context.Request.Url.AbsolutePath;

            var path = _baseDirectory +
                       requestedPath.Substring(_uriPrefix.EndsWith("/")
                           ? _uriPrefix.Length - 1
                           : _uriPrefix.Length);

            var extension = Path.GetExtension(path);

            if (File.Exists(path))
            {
                switch (extension)
                {
                    case ".ts":
                        CompileTypeScript(context, path);
                        break;

                    default:
                        ReturnFileContent(context, path);
                        break;
                }
            }
            else
            {
                context.Response.StatusCode = 404;

                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.Write("404 - Not found");
                }
            }

            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        private bool IsAuthenticatedUser(HttpListenerContext context)
        {
            if (_httpListener.AuthenticationSchemes == AuthenticationSchemes.None)
                return true;

            if (_credentials == null)
                return true;

            var id = (HttpListenerBasicIdentity) context.User.Identity;
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

        private void CompileTypeScript(HttpListenerContext context, string path)
        {
            var typescriptFile = _typeScriptCompiler.Compile(path);

            ReturnFileContent(context, typescriptFile);
        }

        private void ReturnFileContent(HttpListenerContext context, string path)
        {
            var extension = Path.GetExtension(path);

            context.Response.ContentType = MimeTypes.ContainsKey(extension) ? MimeTypes[extension] : "text/plain";
            context.Response.StatusCode = 200;

            using (var reader = new StreamReader(path))
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.Write(reader.ReadToEnd());
            }
        }
    }
}