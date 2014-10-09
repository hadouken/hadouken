using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.Http;

namespace Hadouken.Core
{
    public class HadoukenService : IHadoukenService
    {
        private readonly ILogger<HadoukenService> _logger;
        private readonly IMigrator _migrator;
        private readonly ISessionHandler _sessionHandler;
        private readonly IHttpServer _httpServer;
        private readonly IList<IPlugin> _plugins;

        public HadoukenService(ILogger<HadoukenService> logger,
            IMigrator migrator,
            ISessionHandler sessionHandler,
            IHttpServer httpServer,
            IEnumerable<IPlugin> plugins)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (migrator == null) throw new ArgumentNullException("migrator");
            if (sessionHandler == null) throw new ArgumentNullException("sessionHandler");
            if (httpServer == null) throw new ArgumentNullException("httpServer");

            _logger = logger;
            _migrator = migrator;
            _sessionHandler = sessionHandler;
            _httpServer = httpServer;
            _plugins = new List<IPlugin>(plugins);
        }

        public void Load(string[] args)
        {
            _migrator.Migrate();

            _logger.Info("Loading BitTorrent session.");
            _sessionHandler.Load();

            foreach (var plugin in _plugins)
            {
                _logger.Info("Loading plugin {PluginId}", plugin.GetId());
                plugin.Load();
            }

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