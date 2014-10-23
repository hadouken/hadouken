using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Hadouken.Core.BitTorrent.Data;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class ChangeTorrentLabelHandler : IMessageHandler<ChangeTorrentLabelMessage>
    {
        private readonly ITorrentManager _torrentManager;
        private readonly ITorrentMetadataRepository _metadataRepository;
        private readonly IMessageBus _messageBus;

        public ChangeTorrentLabelHandler(ITorrentManager torrentManager,
            ITorrentMetadataRepository metadataRepository,
            IMessageBus messageBus)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            if (metadataRepository == null) throw new ArgumentNullException("metadataRepository");
            if (messageBus == null) throw new ArgumentNullException("messageBus");

            _torrentManager = torrentManager;
            _metadataRepository = metadataRepository;
            _messageBus = messageBus;
        }

        public void Handle(ChangeTorrentLabelMessage message)
        {
            _metadataRepository.SetLabel(message.InfoHash, message.Label);

            Torrent torrent;
            if (_torrentManager.Torrents.TryGetValue(message.InfoHash, out torrent))
            {
                torrent.Label = message.Label;
            }

            _messageBus.Publish(new TorrentLabelChangedMessage(torrent));
        }
    }
}
