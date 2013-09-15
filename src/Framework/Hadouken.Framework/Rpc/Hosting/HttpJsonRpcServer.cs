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
            try
            {
                var context = _httpListener.EndGetContext(ar);
                Task.Run(() => ProcessContext(context));
            }
            catch (ObjectDisposedException)
            {
                return;
            }

            _httpListener.BeginGetContext(GetContext, null);
        }

        private void ProcessContext(HttpListenerContext context)
        {
            using (var reader = new StreamReader(context.Request.InputStream))
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                var content = reader.ReadToEnd();

                try
                {
                    var response = _rpcHandler.HandleAsync(content).Result;

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
    }
}
