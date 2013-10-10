using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hadouken.Framework.Events;
using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Rpc
{
    public interface IHttpJsonRpcServer : IJsonRpcServer
    {
        void SetCredentials(string userName, string password);
    }

    public class AuthChangedEventArgs
    {
        public string UserName { get; set; }

        public string HashedPassword { get; set; }
    }

    public class HttpJsonRpcServer : IHttpJsonRpcServer
    {
        private readonly IEventListener _eventListener;
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private readonly Task _workerTask;
        private readonly Lazy<IWcfRpcService> _rpcProxy;
        private NetworkCredential _credentials = null;

        public HttpJsonRpcServer(string listenUri, IEventListener eventListener)
        {
            _eventListener = eventListener;
            _rpcProxy = new Lazy<IWcfRpcService>(BuildProxy);
            _httpListener.Prefixes.Add(listenUri);
            _workerTask = new Task(ct => Run(_cancellationToken.Token), _cancellationToken.Token);
        }

        private IWcfRpcService BuildProxy()
        {
            var binding = new NetNamedPipeBinding
            {
                MaxBufferPoolSize = 10485760,
                MaxBufferSize = 10485760,
                MaxConnections = 10,
                MaxReceivedMessageSize = 10485760
            };

            // Create proxy
            var factory = new ChannelFactory<IWcfRpcService>(binding, "net.pipe://localhost/hdkn.jsonrpc");
            return factory.CreateChannel();
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

            while (!_cancellationToken.IsCancellationRequested)
            {
                var context = await _httpListener.GetContextAsync();

                if (cancellationToken.IsCancellationRequested)
                    break;

                ProcessContext(context);
            }

            _httpListener.Close();
        }

        private async void ProcessContext(HttpListenerContext context)
        {
            if (!IsAuthenticatedUser(context))
            {
                context.Response.StatusCode = 401;
                context.Response.OutputStream.Close();
                context.Response.Close();

                return;
            }

            using (var reader = new StreamReader(context.Request.InputStream))
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                var content = await reader.ReadToEndAsync();

                try
                {
                    var response = _rpcProxy.Value.Call(content);

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = 200;

                    writer.Write(response);
                }
                catch (Exception e)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = 500;

                    writer.Write(e.ToString());
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
    }
}
