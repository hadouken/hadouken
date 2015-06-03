using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class QueuePositionTopHandler : IMessageHandler<QueuePositionTopMessage> {
        private readonly ITorrentManager _torrentManager;

        public QueuePositionTopHandler(ITorrentManager torrentManager) {
            if (torrentManager == null) {
                throw new ArgumentNullException("torrentManager");
            }
            this._torrentManager = torrentManager;
        }

        public void Handle(QueuePositionTopMessage message) {
            Torrent torrent;
            if (!this._torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) {
                return;
            }

            torrent.Handle.QueuePositionTop();
        }
    }
}