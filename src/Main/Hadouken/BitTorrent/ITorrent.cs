using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITorrent
    {
        List<List<string>> AnnounceUrls { get; }
        string Comment { get; }
        string CreatedBy { get; }
        DateTime CreationDate { get; }
        byte[] ED2K { get; }
        string Encoding { get; }
        List<string> GetRightHttpSeeds { get; }
        string InfoHash { get; }
        bool IsPrivate { get; }
        string Name { get; }
        int PieceLength { get; }
        string Publisher { get; }
        string PublisherUrl { get; }
        byte[] SHA1 { get; }
        long Size { get; }
        string Source { get; }

        ITorrentFile[] Files { get; }
    }
}
