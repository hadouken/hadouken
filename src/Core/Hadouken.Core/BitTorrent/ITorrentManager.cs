using System.Collections.Generic;

namespace Hadouken.Core.BitTorrent {
    internal interface ITorrentManager {
        IDictionary<string, Torrent> Torrents { get; }
        void Load();
        void Unload();
    }
}