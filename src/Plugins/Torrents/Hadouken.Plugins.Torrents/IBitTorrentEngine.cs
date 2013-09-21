using System.Collections.Generic;
using MonoTorrent.Client;

namespace Hadouken.Plugins.Torrents
{
    public interface IBitTorrentEngine
    {
        void Load();
        void Unload();

        IEnumerable<TorrentManager> TorrentManagers { get; }

        TorrentManager Get(string infoHash);

        TorrentManager Add(byte[] data, string savePath = null, string label = null);

        void Delete(string infoHash);

        void Move(string infoHash, string targetPath);
    }
}
