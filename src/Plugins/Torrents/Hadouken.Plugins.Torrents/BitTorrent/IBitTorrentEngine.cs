using System.Collections.Generic;
namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public interface IBitTorrentEngine
    {
        void Load();
        void Unload();

        IEnumerable<IExtendedTorrentManager> Managers { get; }
        IExtendedTorrentManager Get(string infoHash);
        IExtendedTorrentManager Add(byte[] data, string savePath = null, string label = null, bool propagateEvent = true, long uploadedBytes = 0, long downloadedBytes = 0);
        void Remove(IExtendedTorrentManager manager);
    }
}
