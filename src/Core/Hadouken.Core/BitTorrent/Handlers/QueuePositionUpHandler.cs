using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class QueuePositionUpHandler : IMessageHandler<QueuePositionUpMessage>
    {
        private readonly ITorrentManager _torrentManager;

        public QueuePositionUpHandler(ITorrentManager torrentManager)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            _torrentManager = torrentManager;
        }

        public void Handle(QueuePositionUpMessage message)
        {
            Torrent torrent;
            if (!_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) return;

            torrent.Handle.QueuePositionUp();
        }
    }
}
