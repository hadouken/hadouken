using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;

namespace Hadouken.Core.Http
{
    public class HttpServer : IHttpServer
    {
        private readonly INancyBootstrapper _bootstrapper;
        private NancyHost _nancyHost;

        public HttpServer(INancyBootstrapper bootstrapper)
        {
            if (bootstrapper == null) throw new ArgumentNullException("bootstrapper");
            _bootstrapper = bootstrapper;
        }

        public void Start()
        {
            var cfg = new HostConfiguration
            {
                RewriteLocalhost = false
            };

            _nancyHost = new NancyHost(_bootstrapper, cfg, new Uri("http://localhost:7890/"));
            _nancyHost.Start();
        }

        public void Stop()
        {
            _nancyHost.Stop();
            _nancyHost.Dispose();
        }
    }
}