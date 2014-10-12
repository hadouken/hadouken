using System.Collections.Generic;

namespace Hadouken.Core.BitTorrent
{
    internal interface ITorrentManager
    {
        void Load();

        void Unload();

        IDictionary<string, Torrent> Torrents { get; } 
    }
}
