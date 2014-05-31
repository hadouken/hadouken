using System;
using System.Collections.Generic;
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
            var binding = _configuration.HostBinding;
            var port = _configuration.Port;
            var uriList = new List<Uri> {new Uri(string.Format("http://localhost:{0}/", port))};

            if (!binding.Equals("+") && !binding.Equals("localhost"))
            {
                uriList.Add(new Uri(string.Format("http://{0}:{1}/", binding, port)));
            }

            var cfg = new HostConfiguration
            {
                RewriteLocalhost = binding.Equals("+")
            };

            _httpServer = new NancyHost(new CustomNancyBootstrapper(_lifetimeScope), cfg, uriList.ToArray());
            _httpServer.Start();
        }

        public void Stop()
        {
            _httpServer.Stop();
            _httpServer.Dispose();
        }
    }
}