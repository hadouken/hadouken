using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class TorrentAddedMessage : IMessage {
        private readonly ITorrent _torrent;

        public TorrentAddedMessage(ITorrent torrent) {
            if (torrent == null) {
                throw new ArgumentNullException("torrent");
            }
            this._torrent = torrent;
        }

        public ITorrent Torrent {
            get { return this._torrent; }
        }
    }
}