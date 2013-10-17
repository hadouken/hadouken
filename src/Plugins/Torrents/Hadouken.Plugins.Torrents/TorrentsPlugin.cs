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
        
        public TorrentsPlugin(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override void OnStart()
        {
            _torrentEngine.Load();
        }

        public override void OnStop()
        {
            _torrentEngine.Unload();
        }
    }
}
