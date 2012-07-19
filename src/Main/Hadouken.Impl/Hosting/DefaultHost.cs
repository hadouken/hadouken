using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Hosting;
using Hadouken.Http;
using Hadouken.Data;
using Hadouken.Plugins;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Hosting
{
    public class DefaultHost : IHost
    {
        private IDataRepository _data;
        private IBitTorrentEngine _torrentEngine;
        private IMigrationRunner _migratorRunner;
        private IPluginEngine _pluginEngine;
        private IHttpServer _httpServer;

        public DefaultHost(IDataRepository data, IBitTorrentEngine torrentEngine, IMigrationRunner runner, IPluginEngine pluginEngine, IHttpServer httpServer)
        {
            _data = data;
            _torrentEngine = torrentEngine;
            _migratorRunner = runner;
            _pluginEngine = pluginEngine;
            _httpServer = httpServer;
        }

        public void Load()
        {
            _migratorRunner.Up(this.GetType().Assembly);

            _torrentEngine.Load();

            _pluginEngine.Refresh();

            _httpServer.Start();
        }

        public void Unload()
        {
            _httpServer.Stop();

            _pluginEngine.UnloadAll();

            _torrentEngine.Unload();
        }
    }
}
