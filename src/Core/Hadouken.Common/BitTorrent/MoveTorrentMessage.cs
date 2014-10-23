using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class MoveTorrentMessage : IMessage
    {
        private readonly string _infoHash;
        private readonly string _destination;

        public MoveTorrentMessage(string infoHash, string destination)
        {
            if (infoHash == null) throw new ArgumentNullException("infoHash");
            if (destination == null) throw new ArgumentNullException("destination");
            _infoHash = infoHash;
            _destination = destination;
        }

        public string InfoHash
        {
            get { return _infoHash; }
        }

        public string Destination
        {
            get { return _destination; }
        }

        public bool OverwriteExisting { get; set; }
    }
}
