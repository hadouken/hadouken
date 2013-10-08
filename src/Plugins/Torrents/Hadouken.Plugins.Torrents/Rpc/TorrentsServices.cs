using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Torrents.BitTorrent;
using Hadouken.Plugins.Torrents.Rpc.Dto;

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

        [JsonRpcMethod("torrents.list")]
        public IEnumerable<TorrentOverview> List()
        {
            var torrents = _torrentEngine.TorrentManagers;
            return torrents.Select(t => new TorrentOverview(t));
        }

        [JsonRpcMethod("torrents.details")]
        public TorrentDetails Details(string id)
        {
            var manager = _torrentEngine.Get(id);

            if (manager == null)
                return null;

            return new TorrentDetails(manager);
        }

        [JsonRpcMethod("torrents.addFile")]
        public TorrentOverview AddFile(byte[] data, string savePath, string label)
        {
            var manager = _torrentEngine.Add(data, savePath, label);

            if (manager == null)
                return null;

            return new TorrentOverview(manager);
        }
    }
}
