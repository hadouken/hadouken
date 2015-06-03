using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Hadouken.Core.BitTorrent.Data;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class ChangeTorrentLabelHandler : IMessageHandler<ChangeTorrentLabelMessage> {
        private readonly IMessageBus _messageBus;
        private readonly ITorrentMetadataRepository _metadataRepository;
        private readonly ITorrentManager _torrentManager;

        public ChangeTorrentLabelHandler(ITorrentManager torrentManager,
            ITorrentMetadataRepository metadataRepository,
            IMessageBus messageBus) {
            if (torrentManager == null) {
                throw new ArgumentNullException("torrentManager");
            }
            if (metadataRepository == null) {
                throw new ArgumentNullException("metadataRepository");
            }
            if (messageBus == null) {
                throw new ArgumentNullException("messageBus");
            }

            this._torrentManager = torrentManager;
            this._metadataRepository = metadataRepository;
            this._messageBus = messageBus;
        }

        public void Handle(ChangeTorrentLabelMessage message) {
            this._metadataRepository.SetLabel(message.InfoHash, message.Label);

            Torrent torrent;
            if (this._torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent)) {
                torrent.Label = message.Label;
            }

            this._messageBus.Publish(new TorrentLabelChangedMessage(torrent));
        }
    }
}