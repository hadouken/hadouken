using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.Http;

namespace Hadouken.Core
{
    public class HadoukenService : IHadoukenService
    {
        private readonly ILogger _logger;
        private readonly ISessionHandler _sessionHandler;
        private readonly IHttpServer _httpServer;
        private readonly IEnumerable<IPlugin> _plugins;

        public HadoukenService(ILogger logger,
            ISessionHandler sessionHandler,
            IHttpServer httpServer,
            IEnumerable<IPlugin> plugins)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (sessionHandler == null) throw new ArgumentNullException("sessionHandler");
            if (httpServer == null) throw new ArgumentNullException("httpServer");

            _logger = logger;
            _sessionHandler = sessionHandler;
            _httpServer = httpServer;
            _plugins = plugins ?? Enumerable.Empty<IPlugin>();
        }

        public void Load(string[] args)
        {
            _logger.Info("Loading BitTorrent session.");

            _sessionHandler.Load();

            _logger.Info("Loading plugins.");

            foreach (var plugin in _plugins)
            {
                plugin.Load();
            }

            _logger.Info("Starting HTTP server.");

            _httpServer.Start();
        }

        public void Unload()
        {
            _logger.Info("Stopping HTTP server.");

            _httpServer.Stop();

            _logger.Info("Unloading plugins.");

            foreach (var plugin in _plugins)
            {
                plugin.Unload();
            }

            _logger.Info("Unloading BitTorrent session.");

            _sessionHandler.Unload();
        }
    }
}