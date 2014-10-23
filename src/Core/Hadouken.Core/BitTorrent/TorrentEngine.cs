using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Common.BitTorrent;
using Hadouken.Core.BitTorrent.Data;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class TorrentEngine : ITorrentEngine
    {
        private readonly ITorrentManager _torrentManager;
        private readonly ITorrentMetadataRepository _torrentMetadataRepository;

        public TorrentEngine(ITorrentManager torrentManager,
            ITorrentMetadataRepository torrentMetadataRepository)
        {
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");
            if (torrentMetadataRepository == null) throw new ArgumentNullException("torrentMetadataRepository");
            _torrentManager = torrentManager;
            _torrentMetadataRepository = torrentMetadataRepository;
        }

        public IEnumerable<ITorrent> GetAll()
        {
            return new List<ITorrent>(_torrentManager.Torrents.Values);
        }

        public ITorrent GetByInfoHash(string infoHash)
        {
            Torrent torrent;
            return !_torrentManager.Torrents.TryGetValue(infoHash, out torrent) ? null : torrent;
        }

        public IEnumerable<string> GetLabels()
        {
            return _torrentMetadataRepository.GetAllLabels();
        }
    }
}
