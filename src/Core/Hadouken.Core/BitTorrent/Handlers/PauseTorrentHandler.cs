using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class PauseTorrentHandler : IMessageHandler<PauseTorrentMessage>
    {
        private readonly ITorrentManager _torrentManager;
        private readonly IMessageBus _messageBus;

        public PauseTorrentHandler(ITorrentManager torrentManager,
            IMessageBus messageBus)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            _torrentManager = torrentManager;
            _messageBus = messageBus;
        }

        public void Handle(PauseTorrentMessage message)
        {
            Torrent torrent;
            if (!_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) return;

            torrent.Handle.AutoManaged = false;

            if (torrent.Handle.IsPaused)
            {
                // If the torrent was paused because we took it out of lt queuing
                // we have to send the paused message ourselves.

                _messageBus.Publish(new TorrentPausedMessage(torrent));
            }
            else
            {
                torrent.Handle.Pause();
            }
        }
    }
}
