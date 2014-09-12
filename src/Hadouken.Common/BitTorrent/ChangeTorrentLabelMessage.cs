using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class ChangeTorrentLabelMessage : IMessage
    {
        private readonly string _infoHash;

        public ChangeTorrentLabelMessage(string infoHash)
        {
            if (infoHash == null) throw new ArgumentNullException("infoHash");
            _infoHash = infoHash;
        }

        public string InfoHash
        {
            get { return _infoHash; }
        }

        public string Label { get; set; }
    }
}
