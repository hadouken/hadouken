using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.Common.Http
{
    public abstract class HttpServer : IHttpServer
    {
        private Action<IHttpContext> _requestCallback;

        public abstract void Start(Uri binding, NetworkCredential credential);
        public abstract void Stop();

        public void SetRequestCallback(Action<IHttpContext> requestCallback)
        {
            if(requestCallback == null)
                throw new ArgumentNullException("requestCallback");

            _requestCallback = requestCallback;
        }

        protected void OnHttpRequest(IHttpContext context)
        {
            if (_requestCallback != null)
                _requestCallback(context);
        }
    }
}
