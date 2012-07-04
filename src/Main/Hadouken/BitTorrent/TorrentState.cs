using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.BitTorrent
{
    public enum TorrentState
    {
        Stopped = 0,
        Paused = 1,
        Downloading = 2,
        Seeding = 3,
        Hashing = 4,
        Stopping = 5,
        Error = 6,
        Metadata = 7,
    }
}
