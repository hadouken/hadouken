using System;
using System.Collections.Generic;
using Hadouken.Common.Data;
using Hadouken.Common.Logging;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;

namespace Hadouken.Core.Http
{
    public class HttpServer : IHttpServer
    {
        private readonly ILogger<HttpServer> _logger;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly IKeyValueStore _keyValueStore;
        private NancyHost _nancyHost;

        public HttpServer(ILogger<HttpServer> logger, INancyBootstrapper bootstrapper, IKeyValueStore keyValueStore)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (bootstrapper == null) throw new ArgumentNullException("bootstrapper");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _logger = logger;
            _bootstrapper = bootstrapper;
            _keyValueStore = keyValueStore;
        }

        public void Start()
        {
            var binding = _keyValueStore.Get("http.binding", "localhost");
            var port = _keyValueStore.Get("http.port", 7890);

            var cfg = new HostConfiguration
            {
                RewriteLocalhost = false
            };

            // If binding == '+', rewrite localhost and change binding to localhost
            if (binding == "+")
            {
                cfg.RewriteLocalhost = true;
                binding = "localhost";
            }

            var prefixes = new List<Uri>()
            {
                new Uri(string.Format("http://{0}:{1}/", binding, port))
            };

            // Safeguard if some address is wrong
            if (binding != "localhost")
            {
                prefixes.Add(new Uri("http://localhost:" + port + "/"));
            }

            _nancyHost = new NancyHost(_bootstrapper, cfg, prefixes.ToArray());

            try
            {
                _nancyHost.Start();
                _logger.Info("HTTP server accepting connections on {Prefixes}.", prefixes);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Failed to start HTTP server.");
            }
        }

        public void Stop()
        {
            _nancyHost.Stop();
            _nancyHost.Dispose();
        }
    }
}