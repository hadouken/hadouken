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
        private IMigratorRunner _migratorRunner;
        private IPluginFactory _pluginFactory;
        private IHttpServer _httpServer;

        public DefaultHost(IDataRepository data, IBitTorrentEngine torrentEngine, IMigratorRunner runner, IPluginFactory pluginFactory, IHttpServer httpServer)
        {
            _data = data;
            _torrentEngine = torrentEngine;
            _migratorRunner = runner;
            _pluginFactory = pluginFactory;
            _httpServer = httpServer;
        }

        public void Load()
        {
            _migratorRunner.Run(this.GetType().Assembly);

            _torrentEngine.Load();

            _pluginFactory.LoadAll();

            _httpServer.Start();
        }

        public void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
