using System;
using System.Text.RegularExpressions;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Data;
using Hadouken.Common.Messaging;
using Hadouken.Common.Text;
using Hadouken.Core.BitTorrent.Data;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class AddUrlHandler : IMessageHandler<AddUrlMessage> {
        private readonly IKeyValueStore _keyValueStore;
        private readonly ITorrentMetadataRepository _metadataRepository;
        private readonly ISession _session;

        public AddUrlHandler(ISession session,
            IKeyValueStore keyValueStore,
            ITorrentMetadataRepository metadataRepository) {
            if (session == null) {
                throw new ArgumentNullException("session");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (metadataRepository == null) {
                throw new ArgumentNullException("metadataRepository");
            }

            this._session = session;
            this._keyValueStore = keyValueStore;
            this._metadataRepository = metadataRepository;
        }

        public void Handle(AddUrlMessage message) {
            using (var addParams = new AddTorrentParams()) {
                if (!string.IsNullOrEmpty(message.Name)) {
                    addParams.Name = message.Name;
                }

                addParams.SavePath = message.SavePath ?? this._keyValueStore.Get<string>("bt.save_path");
                addParams.Url = message.Url;

                if (message.Trackers != null) {
                    foreach (var tracker in message.Trackers) {
                        addParams.Trackers.Add(tracker);
                    }
                }

                // Parse info hash
                var infoHash = Regex.Match(message.Url, "urn:btih:([\\w]{32,40})").Groups[1].Value.ToLowerInvariant();

                if (infoHash.Length == 32) {
                    infoHash =
                        BitConverter.ToString(Base32Encoder.ToBytes(infoHash)).Replace("-", "").ToLowerInvariant();
                }

                // Set label
                this._metadataRepository.SetLabel(infoHash, message.Label);

                // Add torrent
                this._session.AsyncAddTorrent(addParams);
            }
        }
    }
}