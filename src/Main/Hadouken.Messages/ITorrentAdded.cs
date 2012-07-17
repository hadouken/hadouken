using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Messaging;
using Hadouken.BitTorrent;

namespace Hadouken.Messages
{
    public interface ITorrentAdded : IMessage
    {
        ITorrentManager TorrentManager { get; set; }
    }
}
