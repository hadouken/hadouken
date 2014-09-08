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
        private readonly ILogger<HadoukenService> _logger;
        private readonly ISessionHandler _sessionHandler;
        private readonly IHttpServer _httpServer;
        private readonly IExtensionFactory _extensionFactory;
        private readonly IList<IPlugin> _plugins;

        public HadoukenService(ILogger<HadoukenService> logger,
            ISessionHandler sessionHandler,
            IHttpServer httpServer,
            IExtensionFactory extensionFactory)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (sessionHandler == null) throw new ArgumentNullException("sessionHandler");
            if (httpServer == null) throw new ArgumentNullException("httpServer");
            if (extensionFactory == null) throw new ArgumentNullException("extensionFactory");

            _logger = logger;
            _sessionHandler = sessionHandler;
            _httpServer = httpServer;
            _extensionFactory = extensionFactory;
            _plugins = new List<IPlugin>();
        }

        public void Load(string[] args)
        {
            _logger.Info("Loading BitTorrent session.");

            _sessionHandler.Load();

            _logger.Info("Loading plugins.");

            foreach (var plugin in _extensionFactory.GetAll<IPlugin>()
                .Where(e => _extensionFactory.IsEnabled(e.GetId())))
            {
                plugin.Load();
                _plugins.Add(plugin);
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