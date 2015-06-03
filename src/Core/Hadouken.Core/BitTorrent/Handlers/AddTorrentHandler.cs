using System;
using Hadouken.Common;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Core.BitTorrent.Data;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers {
    internal sealed class AddTorrentHandler : IMessageHandler<AddTorrentMessage> {
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ITorrentMetadataRepository _metadataRepository;
        private readonly ISession _session;
        private readonly ITorrentInfoRepository _torrentInfoRepository;

        public AddTorrentHandler(IEnvironment environment,
            IFileSystem fileSystem,
            ISession session,
            IKeyValueStore keyValueStore,
            ITorrentMetadataRepository metadataRepository,
            ITorrentInfoRepository torrentInfoRepository) {
            if (environment == null) {
                throw new ArgumentNullException("environment");
            }
            if (fileSystem == null) {
                throw new ArgumentNullException("fileSystem");
            }
            if (session == null) {
                throw new ArgumentNullException("session");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (metadataRepository == null) {
                throw new ArgumentNullException("metadataRepository");
            }
            if (torrentInfoRepository == null) {
                throw new ArgumentNullException("torrentInfoRepository");
            }

            this._environment = environment;
            this._fileSystem = fileSystem;
            this._session = session;
            this._keyValueStore = keyValueStore;
            this._metadataRepository = metadataRepository;
            this._torrentInfoRepository = torrentInfoRepository;
        }

        public void Handle(AddTorrentMessage message) {
            using (var addParams = new AddTorrentParams()) {
                addParams.SavePath = message.SavePath ?? this._keyValueStore.Get<string>("bt.save_path");
                addParams.TorrentInfo = new TorrentInfo(message.Data);

                // Set default settings
                addParams.MaxConnections = this._keyValueStore.Get("bt.defaults.max_connections", 200);
                addParams.MaxUploads = this._keyValueStore.Get("bt.defaults.max_uploads", 10);
                addParams.DownloadLimit = this._keyValueStore.Get("bt.defaults.download_limit", 0);
                addParams.UploadLimit = this._keyValueStore.Get("bt.defaults.upload_limit", 0);

                // Save torrent info
                this._torrentInfoRepository.Save(addParams.TorrentInfo);

                // Load existing resume data if we have any
                var resumePath = this._environment.GetApplicationDataPath()
                    .Combine("Torrents")
                    .CombineWithFilePath(addParams.TorrentInfo.InfoHash + ".resume");
                var resumeFile = this._fileSystem.GetFile(resumePath);

                if (resumeFile.Exists) {
                    using (var stream = resumeFile.OpenRead()) {
                        var resumeData = new byte[stream.Length];
                        stream.Read(resumeData, 0, resumeData.Length);

                        addParams.ResumeData = resumeData;
                    }
                }

                // Save metadata
                this._metadataRepository.SetLabel(addParams.TorrentInfo.InfoHash, message.Label);

                // Add torrent
                this._session.AsyncAddTorrent(addParams);
            }
        }
    }
}