using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class MoveTorrentHandler : IMessageHandler<MoveTorrentMessage>
    {
        private readonly ITorrentManager _torrentManager;

        public MoveTorrentHandler(ITorrentManager torrentManager)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            _torrentManager = torrentManager;
        }

        public void Handle(MoveTorrentMessage message)
        {
            Torrent torrent;
            if (!_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) return;

            var flags = message.OverwriteExisting
                    ? TorrentHandle.MoveFlags.AlwaysReplaceFiles
                    : TorrentHandle.MoveFlags.DontReplace;

            torrent.Handle.MoveStorage(message.Destination, flags);
        }
    }
}
