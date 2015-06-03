using System;
using System.Collections.Generic;
using Hadouken.Common.BitTorrent;
using Hadouken.Core.BitTorrent.Data;

namespace Hadouken.Core.BitTorrent {
    internal sealed class TorrentEngine : ITorrentEngine {
        private readonly ITorrentManager _torrentManager;
        private readonly ITorrentMetadataRepository _torrentMetadataRepository;

        public TorrentEngine(ITorrentManager torrentManager,
            ITorrentMetadataRepository torrentMetadataRepository) {
            if (torrentManager == null) {
                throw new ArgumentNullException("torrentManager");
            }
            if (torrentMetadataRepository == null) {
                throw new ArgumentNullException("torrentMetadataRepository");
            }
            this._torrentManager = torrentManager;
            this._torrentMetadataRepository = torrentMetadataRepository;
        }

        public IEnumerable<ITorrent> GetAll() {
            return new List<ITorrent>(this._torrentManager.Torrents.Values);
        }

        public ITorrent GetByInfoHash(string infoHash) {
            Torrent torrent;
            return !this._torrentManager.Torrents.TryGetValue(infoHash, out torrent) ? null : torrent;
        }

        public IEnumerable<string> GetLabels() {
            return this._torrentMetadataRepository.GetAllLabels();
        }
    }
}