using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Rpc
{
    public interface IHttpJsonRpcServer : IJsonRpcServer
    {
    }

    public class HttpJsonRpcServer : IHttpJsonRpcServer
    {
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly NetworkCredential _credentials;
        private readonly CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private readonly Task _workerTask;
        private readonly Lazy<IWcfJsonRpcServer> _rpcProxy; 

        public HttpJsonRpcServer(string listenUri, NetworkCredential credentials = null)
        {
            _rpcProxy = new Lazy<IWcfJsonRpcServer>(BuildProxy);
            _httpListener.Prefixes.Add(listenUri);
            _credentials = credentials;
            _workerTask = new Task(ct => Run(_cancellationToken.Token), _cancellationToken.Token);
        }

        private IWcfJsonRpcServer BuildProxy()
        {
            var binding = new NetNamedPipeBinding
            {
                MaxBufferPoolSize = 10485760,
                MaxBufferSize = 10485760,
                MaxConnections = 10,
                MaxReceivedMessageSize = 10485760
            };

            // Create proxy
            var factory = new ChannelFactory<IWcfJsonRpcServer>(binding, "net.pipe://localhost/hdkn.jsonrpc");
            return factory.CreateChannel();
        }

        public void Open()
        {
            if (_credentials != null)
            {
                _httpListener.AuthenticationSchemes = AuthenticationSchemes.Basic;
            }

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
