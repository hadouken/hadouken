using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITorrentFile
    {
        IBitField BitField { get; }
        long BytesDownloaded { get; }
        byte[] ED2K { get; }
        int EndPieceIndex { get; }
        long Length { get; }
        byte[] MD5 { get; }
        string Path { get; }
        Priority Priority { get; set; }
        byte[] SHA1 { get; }
        int StartPieceIndex { get; }
    }
}
