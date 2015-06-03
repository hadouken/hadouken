﻿using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class ChangeFilePriorityHandler : IMessageHandler<ChangeFilePriorityMessage> {
        private readonly ITorrentManager _torrentManager;

        public ChangeFilePriorityHandler(ITorrentManager torrentManager) {
            if (torrentManager == null) {
                throw new ArgumentNullException("torrentManager");
            }
            this._torrentManager = torrentManager;
        }

        public void Handle(ChangeFilePriorityMessage message) {
            Torrent torrent;
            if (!this._torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) {
                return;
            }

            torrent.Handle.SetFilePriority(message.FileIndex, message.Priority);
        }
    }
}