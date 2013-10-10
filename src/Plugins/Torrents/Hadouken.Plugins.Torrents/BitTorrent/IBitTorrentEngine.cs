using System.Collections.Generic;
namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public interface IBitTorrentEngine
    {
        void Load();
        void Unload();

        IEnumerable<IExtendedTorrentManager> Managers { get; }

        IExtendedTorrentManager Get(string infoHash);

        IExtendedTorrentManager Add(byte[] data, string savePath = null, string label = null);

        IExtendedTorrentManager AddMagnetLink(string magnetLink, string savePath, string label);

        void Remove(IExtendedTorrentManager manager);
    }
}
