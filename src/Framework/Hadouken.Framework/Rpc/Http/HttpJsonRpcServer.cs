using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hadouken.Framework.Rpc.Http
{
    public class HttpJsonRpcServer : IJsonRpcServer
    {
        private readonly IRequestBuilder _requestBuilder;
        private readonly IRequestHandler _requestHandler;
        private readonly HttpListener _httpListener = new HttpListener();

        public HttpJsonRpcServer(IUriFactory uriFactory, IRequestBuilder requestBuilder, IRequestHandler requestHandler)
        {
            _httpListener.Prefixes.Add(uriFactory.GetListenUri());

            _requestBuilder = requestBuilder;
            _requestHandler = requestHandler;
        }

        public void Start()
        {
            _httpListener.Start();
            _httpListener.BeginGetContext(GetContext, null);
        }

        public void Stop()
        {
            _httpListener.Stop();
        }

        private void GetContext(IAsyncResult ar)
        {
            var context = _httpListener.EndGetContext(ar);

            using (var ms = new MemoryStream())
            {
                context.Request.InputStream.CopyTo(ms);
                var jsonRequest = Encoding.UTF8.GetString(ms.ToArray());

                // Build request object
                var request = _requestBuilder.Build(jsonRequest);
                var response = _requestHandler.Execute(request);

                using (context.Response.OutputStream)
                {
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

                    context.Response.OutputStream.Write(data, 0, data.Length);
                    context.Response.OutputStream.Flush();
                }

                context.Response.StatusCode = 200;
                context.Response.OutputStream.Close();
                context.Response.Close();
            }

            _httpListener.BeginGetContext(GetContext, null);
        }
    }
}
