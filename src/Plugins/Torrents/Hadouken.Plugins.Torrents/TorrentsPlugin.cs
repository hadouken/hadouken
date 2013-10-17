using System;
using System.ServiceModel;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Framework.Wcf;
using Hadouken.Plugins.Torrents.BitTorrent;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsPlugin : Plugin
    {
        private readonly IBitTorrentEngine _torrentEngine;
        private readonly IServiceHost _rpcServer;

        public TorrentsPlugin(IBitTorrentEngine torrentEngine, IServiceHostFactory<IWcfRpcService> serviceHostFactory)
        {
            _torrentEngine = torrentEngine;
            _rpcServer =
                serviceHostFactory.Create(new Uri("net.pipe://localhost/hdkn.plugins.core.torrents"));
        }

        public override void OnStart()
        {
            _torrentEngine.Load();
            _rpcServer.Open();
        }

        public override void OnStop()
        {
            _rpcServer.Close();
            _torrentEngine.Unload();
        }
    }
}
