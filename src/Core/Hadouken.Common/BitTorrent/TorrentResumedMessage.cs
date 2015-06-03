using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class TorrentResumedMessage : IMessage {
        private readonly ITorrent _torrent;

        public TorrentResumedMessage(ITorrent torrent) {
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