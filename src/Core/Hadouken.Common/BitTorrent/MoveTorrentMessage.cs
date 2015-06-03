using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class MoveTorrentMessage : IMessage {
        private readonly string _destination;
        private readonly string _infoHash;

        public MoveTorrentMessage(string infoHash, string destination) {
            if (infoHash == null) {
                throw new ArgumentNullException("infoHash");
            }
            if (destination == null) {
                throw new ArgumentNullException("destination");
            }
            this._infoHash = infoHash;
            this._destination = destination;
        }

        public string InfoHash {
            get { return this._infoHash; }
        }

        public string Destination {
            get { return this._destination; }
        }

        public bool OverwriteExisting { get; set; }
    }
}