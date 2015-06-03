using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class QueuePositionDownMessage : IMessage {
        private readonly string _infoHash;

        public QueuePositionDownMessage(string infoHash) {
            if (infoHash == null) {
                throw new ArgumentNullException("infoHash");
            }
            this._infoHash = infoHash;
        }

        public string InfoHash {
            get { return this._infoHash; }
        }
    }
}