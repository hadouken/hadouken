using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Hadouken.BitTorrent
{
    public interface IBitTorrentEngine : IComponent
    {
        void Load();
        void Unload();

        IList<ITorrent> Torrents { get; }

        ITorrent AddTorrent(byte[] data, bool autoStart = false, string savePath = "", string tag = "");

        void RemoveTorrent(ITorrent torrent);
        void RemoveTorrent(string infoHash);

        void StartTorrent(ITorrent torrent);
        void StartTorrent(string infoHash);

        void StopTorrent(ITorrent torrent);
        void StopTorrent(string infoHash);

        void PauseTorrent(ITorrent torrent);
        void PauseTorrent(string infoHash);

        void MoveTorrent(ITorrent torrent, string newPath);
        void MoveTorrent(string infoHash, string newPath);
    }
}
