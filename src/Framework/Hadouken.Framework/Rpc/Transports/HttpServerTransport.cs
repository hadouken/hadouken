using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Transports
{
    public class HttpTransport : Transport
    {
        private readonly HttpListener _httpListener = new HttpListener();
        private readonly string _listenUri;
        private readonly IRequestBuilder _requestBuilder;

        public HttpTransport(string listenUri, IRequestBuilder requestBuilder)
        {
            _listenUri = listenUri;
            _requestBuilder = requestBuilder;
        }

        public override bool SupportsScheme(string scheme)
        {
            return String.Equals(scheme, "http", StringComparison.InvariantCultureIgnoreCase);
        }

        public override void Open()
        {
            _httpListener.Prefixes.Add(_listenUri);
            _httpListener.Start();

            _httpListener.BeginGetContext(GetContext, null);
        }

        private void GetContext(IAsyncResult ar)
        {
            var context = _httpListener.EndGetContext(ar);
        }

        public override void Close()
        {
            _httpListener.Stop();
            _httpListener.Close();
        }
    }
}
