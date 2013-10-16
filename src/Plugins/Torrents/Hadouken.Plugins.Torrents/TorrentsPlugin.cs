using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Torrents.BitTorrent;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsPlugin : Plugin
    {
        private readonly IBitTorrentEngine _torrentEngine;
        private readonly IWcfJsonRpcServer _rpcServer;

        public TorrentsPlugin(IBitTorrentEngine torrentEngine, IWcfJsonRpcServer rpcServer)
        {
            _torrentEngine = torrentEngine;
            _rpcServer = rpcServer;
        }

        public override void Load()
        {
            _torrentEngine.Load();
            _rpcServer.Open();
        }

        public override void Unload()
        {
            _rpcServer.Close();
            _torrentEngine.Unload();
        }
    }
}
