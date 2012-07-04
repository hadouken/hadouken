using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public interface ITorrentFile
    {
        string Name { get; }
        long Size { get; }
    }
}
