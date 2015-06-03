using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class ChangeTorrentSettingsHandler : IMessageHandler<ChangeTorrentSettingsMessage> {
        private readonly ITorrentManager _torrentManager;

        public ChangeTorrentSettingsHandler(ITorrentManager torrentManager) {
            if (torrentManager == null) {
                throw new ArgumentNullException("torrentManager");
            }
            this._torrentManager = torrentManager;
        }

        public void Handle(ChangeTorrentSettingsMessage message) {
            Torrent torrent;
            if (!this._torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) {
                return;
            }

            torrent.Handle.DownloadLimit = message.DownloadRateLimit;
            torrent.Handle.MaxConnections = message.MaxConnections;
            torrent.Handle.MaxUploads = message.MaxUploads;
            torrent.Handle.SequentialDownload = message.SequentialDownload;
            torrent.Handle.UploadLimit = message.UploadRateLimit;
        }
    }
}