using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.SelfHost;
using System.Web.Http;

namespace Hadouken.Common.Http.HttpListener
{
    public class SelfHostedWebApiServer : IHttpWebApiServer
    {
        private readonly HttpSelfHostServer _hostServer;

        internal SelfHostedWebApiServer(HttpSelfHostConfiguration configuration)
        {
            _hostServer = new HttpSelfHostServer(configuration);
        }

        public void Start()
        {
            _hostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _hostServer.CloseAsync().Wait();
            _hostServer.Dispose();
        }
    }
}
