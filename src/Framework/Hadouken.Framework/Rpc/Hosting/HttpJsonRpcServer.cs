using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Hosting
{
    public class HttpJsonRpcServer : IJsonRpcServer
    {
        private readonly IJsonRpcHandler _rpcHandler;
        private readonly HttpListener _httpListener = new HttpListener();

        public HttpJsonRpcServer(string listenUri, IJsonRpcHandler rpcHandler)
        {
            _rpcHandler = rpcHandler;
            _httpListener.Prefixes.Add(listenUri);
        }

        public void Open()
        {
            _httpListener.Start();
            _httpListener.BeginGetContext(GetContext, null);
        }

        public void Close()
        {
            _httpListener.Close();
        }

        private void GetContext(IAsyncResult ar)
        {
            var context = _httpListener.EndGetContext(ar);
            Task.Run(() => ProcessContext(context));

            _httpListener.BeginGetContext(GetContext, null);
        }

        private void ProcessContext(HttpListenerContext context)
        {
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                var content = reader.ReadToEnd();
                var response = _rpcHandler.HandleAsync(content).Result;

                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    context.Response.StatusCode = 200;
                    writer.Write(response);
                }
            }

            context.Response.OutputStream.Close();
            context.Response.Close();
        }
    }
}
