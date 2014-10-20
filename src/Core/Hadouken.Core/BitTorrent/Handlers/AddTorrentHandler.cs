using System;
using Hadouken.Common;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Messaging;
using Hadouken.Core.BitTorrent.Data;
using Ragnar;

namespace Hadouken.Core.BitTorrent.Handlers
{
    internal sealed class AddTorrentHandler : IMessageHandler<AddTorrentMessage>
    {
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly ISession _session;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ITorrentMetadataRepository _metadataRepository;
        private readonly ITorrentInfoRepository _torrentInfoRepository;

        public AddTorrentHandler(IEnvironment environment,
            IFileSystem fileSystem,
            ISession session,
            IKeyValueStore keyValueStore,
            ITorrentMetadataRepository metadataRepository,
            ITorrentInfoRepository torrentInfoRepository)
        {
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (session == null) throw new ArgumentNullException("session");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (metadataRepository == null) throw new ArgumentNullException("metadataRepository");
            if (torrentInfoRepository == null) throw new ArgumentNullException("torrentInfoRepository");

            _environment = environment;
            _fileSystem = fileSystem;
            _session = session;
            _keyValueStore = keyValueStore;
            _metadataRepository = metadataRepository;
            _torrentInfoRepository = torrentInfoRepository;
        }

        public void Handle(AddTorrentMessage message)
        {
            using (var addParams = new AddTorrentParams())
            {
                addParams.SavePath = message.SavePath ?? _keyValueStore.Get<string>("bt.save_path");
                addParams.TorrentInfo = new TorrentInfo(message.Data);

                // Save torrent info
                _torrentInfoRepository.Save(addParams.TorrentInfo);

                // Load existing resume data if we have any
                var resumePath = _environment.GetApplicationDataPath()
                    .Combine("Torrents")
                    .CombineWithFilePath(addParams.TorrentInfo.InfoHash + ".resume");
                var resumeFile = _fileSystem.GetFile(resumePath);

                if (resumeFile.Exists)
                {
                    using (var stream = resumeFile.OpenRead())
                    {
                        var resumeData = new byte[stream.Length];
                        stream.Read(resumeData, 0, resumeData.Length);

                        addParams.ResumeData = resumeData;
                    }
                }

                // Save metadata
                _metadataRepository.SetLabel(addParams.TorrentInfo.InfoHash, message.Label);

                // Add torrent
                _session.AsyncAddTorrent(addParams);
            }
        }
    }
}
