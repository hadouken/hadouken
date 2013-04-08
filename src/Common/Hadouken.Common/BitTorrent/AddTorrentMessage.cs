using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public class AddTorrentMessage : Message
    {
        public byte[] Data { get; set; }

        public string Label { get; set; }
    }
}
