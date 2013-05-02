using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public abstract class TorrentMessage : Message
    {
        public string Name { get; set; }
        public string InfoHash { get; set; }
        public long Size { get; set; }
    }
}
