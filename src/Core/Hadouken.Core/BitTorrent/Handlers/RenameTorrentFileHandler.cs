using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class RenameTorrentFileHandler : IMessageHandler<RenameTorrentFileMessage>
    {
        private readonly ITorrentManager _torrentManager;

        public RenameTorrentFileHandler(ITorrentManager torrentManager)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            _torrentManager = torrentManager;
        }

        public void Handle(RenameTorrentFileMessage message)
        {
            Torrent torrent;
            if (!_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) return;

            torrent.Handle.RenameFile(message.FileIndex, message.FileName);
        }
    }
}
