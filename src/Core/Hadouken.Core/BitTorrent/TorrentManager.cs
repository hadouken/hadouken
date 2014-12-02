using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Hadouken.Common;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Core.BitTorrent.Data;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal class TorrentManager : ITorrentManager
    {
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IAlertBus _alertBus;
        private readonly IMessageBus _messageBus;
        private readonly ITorrentInfoRepository _torrentInfoRepository;
        private readonly ITorrentMetadataRepository _torrentMetadataRepository;
        private readonly ConcurrentDictionary<string, Torrent> _torrents;  

        public TorrentManager(IEnvironment environment,
            IFileSystem fileSystem,
            IAlertBus alertBus,
            IMessageBus messageBus,
            ITorrentInfoRepository torrentInfoRepository,
            ITorrentMetadataRepository torrentMetadataRepository)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (alertBus == null) throw new ArgumentNullException("alertBus");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            if (torrentInfoRepository == null) throw new ArgumentNullException("torrentInfoRepository");
            if (torrentMetadataRepository == null) throw new ArgumentNullException("torrentMetadataRepository");

            _environment = environment;
            _fileSystem = fileSystem;
            _alertBus = alertBus;
            _messageBus = messageBus;
            _torrentInfoRepository = torrentInfoRepository;
            _torrentMetadataRepository = torrentMetadataRepository;
            _torrents = new ConcurrentDictionary<string, Torrent>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, Torrent> Torrents
        {
            get { return _torrents; }
        }

        public void Load()
        {
            // Set up alert subscriptions
            _alertBus.Subscribe<MetadataReceivedAlert>(OnMetadataReceived);
            _alertBus.Subscribe<SaveResumeDataAlert>(OnSaveResumeData);
            _alertBus.Subscribe<StateUpdateAlert>(OnStateUpdate);
            _alertBus.Subscribe<TorrentAddedAlert>(OnTorrentAdded);
            _alertBus.Subscribe<TorrentFinishedAlert>(OnTorrentFinished);
            _alertBus.Subscribe<TorrentRemovedAlert>(OnTorrentRemoved);
        }

        public void Unload()
        {
        }

        private void OnMetadataReceived(MetadataReceivedAlert alert)
        {
            using (var handle = alert.Handle)
            {
                Torrent torrent;
                if (!_torrents.TryGetValue(handle.InfoHash.ToHex(), out torrent)) return;

                torrent.TorrentInfo = handle.TorrentFile;
                _torrentInfoRepository.Save(torrent.TorrentInfo);
            }
        }

        private void OnSaveResumeData(SaveResumeDataAlert alert)
        {
            using (var handle = alert.Handle)
            using (var status = handle.QueryStatus())
            {
                var resumePath = _environment.GetApplicationDataPath()
                    .Combine("Torrents")
                    .CombineWithFilePath(string.Concat(status.InfoHash.ToHex(), ".resume"));

                var file = _fileSystem.GetFile(resumePath);

                using (var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fileStream.Write(alert.ResumeData, 0, alert.ResumeData.Length);
                }
            }
        }

        private void OnTorrentAdded(TorrentAddedAlert alert)
        {
            var torrent = new Torrent(alert.Handle);
            torrent.Label = _torrentMetadataRepository.GetLabel(torrent.InfoHash);
            
            if (_torrents.TryAdd(torrent.InfoHash, torrent))
            {
                _messageBus.Publish(new TorrentAddedMessage(torrent));   
            }

            // Save initial resume data
            torrent.Handle.SaveResumeData();
        }

        private void OnTorrentFinished(TorrentFinishedAlert alert)
        {
            using (var handle = alert.Handle)
            {
                Torrent torrent;
                if (!_torrents.TryGetValue(handle.InfoHash.ToHex(), out torrent)) return;

                // Update status
                torrent.Status = handle.QueryStatus();

                // If the downloaded bytes are 0 we probably just added a torrent
                // which was already downloaded. 
                if (torrent.DownloadedBytes == 0)
                {
                    return;
                }

                _messageBus.Publish(new TorrentCompletedMessage(torrent));
            }
        }

        private void OnTorrentRemoved(TorrentRemovedAlert alert)
        {
            Torrent torrent;

            if (_torrents.TryRemove(alert.InfoHash, out torrent))
            {
                torrent.Dispose();                
            }

            _messageBus.Publish(new TorrentRemovedMessage(alert.InfoHash));
        }

        private void OnStateUpdate(StateUpdateAlert alert)
        {
            foreach (var status in alert.Statuses)
            {
                Torrent torrent;
                if (!_torrents.TryGetValue(status.InfoHash.ToHex(), out torrent)) continue;

                torrent.Status = status;
            }
        }
    }
}