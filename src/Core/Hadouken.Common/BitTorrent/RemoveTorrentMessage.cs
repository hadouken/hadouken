using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class RemoveTorrentMessage : IMessage
    {
        private readonly string _infoHash;

        public RemoveTorrentMessage(string infoHash)
        {
            if (infoHash == null) throw new ArgumentNullException("infoHash");
            _infoHash = infoHash;
        }

        public string InfoHash
        {
            get { return _infoHash; }
        }

        public bool RemoveData { get; set; }
    }
}
