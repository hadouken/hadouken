using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class ClearTorrentErrorHandler : IMessageHandler<ClearTorrentErrorMessage>
    {
        private readonly ITorrentManager _torrentManager;

        public ClearTorrentErrorHandler(ITorrentManager torrentManager)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            _torrentManager = torrentManager;
        }

        public void Handle(ClearTorrentErrorMessage message)
        {
            Torrent torrent;
            if (!_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) return;

            torrent.Handle.ClearError();
        }
    }
}
