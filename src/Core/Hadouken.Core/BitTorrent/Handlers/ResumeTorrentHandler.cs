using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class ResumeTorrentHandler : IMessageHandler<ResumeTorrentMessage>
    {
        private readonly ITorrentManager _torrentManager;

        public ResumeTorrentHandler(ITorrentManager torrentManager)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            _torrentManager = torrentManager;
        }

        public void Handle(ResumeTorrentMessage message)
        {
            Torrent torrent;
            if (!_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) return;

            if (torrent.Handle.IsPaused && torrent.Handle.AutoManaged)
            {
                // Cannot resume. Auto managed.
                return;
            }
            
            torrent.Handle.Resume();
        }
    }
}
