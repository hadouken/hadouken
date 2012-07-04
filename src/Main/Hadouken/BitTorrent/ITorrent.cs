using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITorrent
    {
        string Name { get; }
        string InfoHash { get; }
        long Size { get; }
        long DownloadedBytes { get; }
        long UploadedBytes { get; }
        long DownloadSpeed { get; }
        long UploadSpeed { get; }
        double Progress { get; }
        string SavePath { get; }
        string Label { get; }
        TorrentState State { get; }

        bool IsMultiFile { get; }
        
        IList<ITorrentFile> Files { get; }
        IList<IPeer> Peers { get; }
        IList<ITracker> Trackers { get; }
    }
}
