using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Torrents.BitTorrent;

namespace Hadouken.Plugins.Torrents.Rpc
{
    public class TorrentsServices : IJsonRpcService
    {
        private readonly IBitTorrentEngine _torrentEngine;

        public TorrentsServices(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        [JsonRpcMethod("torrents.start")]
        public bool Start(string infoHash)
        {
            var manager = _torrentEngine.Get(infoHash);

            if (manager == null)
                return false;

            manager.Start();

            return true;
        }

        [JsonRpcMethod("torrents.stop")]
        public bool Stop(string infoHash)
        {
            var manager = _torrentEngine.Get(infoHash);

            if (manager == null)
                return false;

            manager.Stop();

            return true;
        }
    }
}
