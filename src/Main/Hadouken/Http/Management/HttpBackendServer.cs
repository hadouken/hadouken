using System;
using Autofac;
using Hadouken.Configuration;
using Nancy.Hosting.Self;

namespace Hadouken.Http.Management
{
    public class HttpBackendServer : IHttpBackendServer
    {
        private readonly IConfiguration _configuration;
        private readonly ILifetimeScope _lifetimeScope;

        private NancyHost _httpServer;

        public HttpBackendServer(IConfiguration configuration, ILifetimeScope lifetimeScope)
        {
            _configuration = configuration;
            _lifetimeScope = lifetimeScope;
        }

        public void Start()
        {
            var uri = new Uri(string.Format("http://localhost:{0}/", _configuration.Http.Port));

            var cfg = new HostConfiguration
            {
                RewriteLocalhost = _configuration.Http.HostBinding.Equals("+")
            };

            _httpServer = new NancyHost(new CustomNancyBootstrapper(_lifetimeScope), cfg, uri);
            _httpServer.Start();
        }

        public void Stop()
        {
            _httpServer.Stop();
            _httpServer.Dispose();
        }
    }
}