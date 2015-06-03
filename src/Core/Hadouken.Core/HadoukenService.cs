using System;
using System.Collections.Generic;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Logging;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.Http;

namespace Hadouken.Core {
    public class HadoukenService : IHadoukenService {
        private readonly IHttpServer _httpServer;
        private readonly ILogger<HadoukenService> _logger;
        private readonly IMigrator _migrator;
        private readonly IList<IPlugin> _plugins;
        private readonly ISessionHandler _sessionHandler;

        public HadoukenService(ILogger<HadoukenService> logger,
            IMigrator migrator,
            ISessionHandler sessionHandler,
            IHttpServer httpServer,
            IEnumerable<IPlugin> plugins) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (migrator == null) {
                throw new ArgumentNullException("migrator");
            }
            if (sessionHandler == null) {
                throw new ArgumentNullException("sessionHandler");
            }
            if (httpServer == null) {
                throw new ArgumentNullException("httpServer");
            }

            this._logger = logger;
            this._migrator = migrator;
            this._sessionHandler = sessionHandler;
            this._httpServer = httpServer;
            this._plugins = new List<IPlugin>(plugins);
        }

        public void Load(string[] args) {
            this._migrator.Migrate();

            this._logger.Info("Loading BitTorrent session.");
            this._sessionHandler.Load();

            foreach (var plugin in this._plugins) {
                this._logger.Info("Loading plugin {PluginId}", plugin.GetId());
                plugin.Load();
            }

            this._httpServer.Start();
        }

        public void Unload() {
            this._logger.Info("Stopping HTTP server.");

            this._httpServer.Stop();

            this._logger.Info("Unloading plugins.");

            foreach (var plugin in this._plugins) {
                plugin.Unload();
            }

            this._logger.Info("Unloading BitTorrent session.");

            this._sessionHandler.Unload();
        }
    }
}