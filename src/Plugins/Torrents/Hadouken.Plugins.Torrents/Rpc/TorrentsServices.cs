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

        [JsonRpcMethod("torrents.list")]
        public object List()
        {
            var torrents = _torrentEngine.TorrentManagers;

            return torrents.Select(t => new
                {
                    id = t.InfoHash.ToString().Replace("-", "").ToLowerInvariant(),
                    name = t.Torrent.Name,
                    size = t.Torrent.Size
                });

        }

        [JsonRpcMethod("torrents.details")]
        public object Details(string id)
        {
            var manager = _torrentEngine.Get(id);

            if (manager == null)
                return null;

            return new
            {
                name = manager.Torrent.Name,
                size = manager.Torrent.Size
            };
        }

        [JsonRpcMethod("torrents.addFile")]
        public object AddFile(byte[] data, string savePath, string label)
        {
            var manager = _torrentEngine.Add(data, savePath, label);
            return new
                {
                    manager.Torrent.Name,
                    manager.Torrent.Size
                };
        }
    }
}
