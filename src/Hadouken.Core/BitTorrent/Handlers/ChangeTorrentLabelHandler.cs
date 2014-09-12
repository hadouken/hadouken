using System;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Hadouken.Core.BitTorrent.Data;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class ChangeTorrentLabelHandler : IMessageHandler<ChangeTorrentLabelMessage>
    {
        private readonly ITorrentEngine _torrentEngine;
        private readonly ITorrentMetadataRepository _metadataRepository;
        private readonly IMessageBus _messageBus;

        public ChangeTorrentLabelHandler(ITorrentEngine torrentEngine,
            ITorrentMetadataRepository metadataRepository,
            IMessageBus messageBus)
        {
            if (torrentEngine == null) throw new ArgumentNullException("torrentEngine");
            if (metadataRepository == null) throw new ArgumentNullException("metadataRepository");
            if (messageBus == null) throw new ArgumentNullException("messageBus");

            _torrentEngine = torrentEngine;
            _metadataRepository = metadataRepository;
            _messageBus = messageBus;
        }

        public void Handle(ChangeTorrentLabelMessage message)
        {
            _metadataRepository.SetLabel(message.InfoHash, message.Label);

            var torrent = _torrentEngine.GetByInfoHash(message.InfoHash);
            _messageBus.Publish(new TorrentLabelChangedMessage(torrent));
        }
    }
}
