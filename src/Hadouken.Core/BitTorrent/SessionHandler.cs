using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Hadouken.Common;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    public class SessionHandler : ISessionHandler, ITorrentEngine
    {
        private readonly ILogger _logger;
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IMessageBus _messageBus;
        private readonly ISession _session;
        private readonly Thread _alertsThread;
        private bool _alertsThreadRunning;

        public SessionHandler(ILogger logger,
            IEnvironment environment,
            IFileSystem fileSystem,
            IMessageBus messageBus,
            ISession session)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            if (session == null) throw new ArgumentNullException("session");

            _logger = logger;
            _environment = environment;
            _fileSystem = fileSystem;
            _messageBus = messageBus;
            _session = session;
            _alertsThread = new Thread(ReadAlerts);
        }

        public void Load()
        {
            // Set up message subscriptions
            _messageBus.Subscribe<AddTorrentMessage>(AddTorrent);
            _messageBus.Subscribe<PauseTorrentMessage>(PauseTorrent);
            _messageBus.Subscribe<ResumeTorrentMessage>(ResumeTorrent);
            _messageBus.Subscribe<RemoveTorrentMessage>(RemoveTorrent);

            // Configure session
            _session.SetAlertMask(SessionAlertCategory.Error
                                  | SessionAlertCategory.Peer
                                  | SessionAlertCategory.Status);
            _session.ListenOn(6881, 6881);

            // Start alerts thread
            _alertsThreadRunning = true;
            _alertsThread.Start();

            // Load previous session state (if any)
            var sessionStatePath = _environment.GetApplicationDataPath().CombineWithFilePath("session_state");
            if (_fileSystem.Exist(sessionStatePath))
            {
                var file = _fileSystem.GetFile(sessionStatePath);

                using (var stateStream = file.OpenRead())
                using (var ms = new MemoryStream())
                {
                    stateStream.CopyTo(ms);
                    _session.LoadState(ms.ToArray());

                    _logger.Debug("Loaded session state.");
                }
            }

            // Load previous torrents (if any)
            var torrentsPath = _environment.GetApplicationDataPath().Combine("Torrents");
            var torrentsDirectory = _fileSystem.GetDirectory(torrentsPath);

            if (!torrentsDirectory.Exists) torrentsDirectory.Create();

            foreach (var file in torrentsDirectory.GetFiles("*.torrent", SearchScope.Current))
            {
                using (var torrentStream = file.OpenRead())
                using (var torrentMemoryStream = new MemoryStream())
                using (var addParams = new AddTorrentParams())
                {
                    torrentStream.CopyTo(torrentMemoryStream);

                    addParams.SavePath = "C:\\Downloads";
                    addParams.TorrentInfo = new TorrentInfo(torrentMemoryStream.ToArray());

                    _logger.Debug("Loading " + addParams.TorrentInfo.Name);

                    // Check resume data
                    var resumePath = file.Path.ChangeExtension(".resume");
                    var resumeFile = _fileSystem.GetFile(resumePath);

                    if (resumeFile.Exists)
                    {
                        _logger.Debug("Loading resume data for " + addParams.TorrentInfo.Name);

                        using (var resumeStream = resumeFile.OpenRead())
                        using (var resumeMemoryStream = new MemoryStream())
                        {
                            resumeStream.CopyTo(resumeMemoryStream);
                            addParams.ResumeData = resumeMemoryStream.ToArray();
                        }
                    }

                    // Add torrent to session
                    _session.AsyncAddTorrent(addParams);
                }
            }
        }

        public void Unload()
        {
            _alertsThreadRunning = false;
            _alertsThread.Join();

            // Save state
            _session.Pause();

            var totalFastResume = 0;

            foreach (var handle in _session.GetTorrents())
            {
                using (handle)
                using (var status = handle.QueryStatus())
                {
                    if (!status.HasMetadata)
                    {
                        _logger.Debug(string.Format("Skipping {0}, no metadata", status.Name));
                        continue;
                    }

                    if (!status.NeedSaveResume)
                    {
                        _logger.Debug(string.Format("Skipping {0}, resume-file up to date", status.Name));
                        continue;
                    }

                    handle.SaveResumeData();
                    totalFastResume += 1;
                }
            }

            _logger.Debug(string.Format("{0} torrents needs to save resume-file", totalFastResume));

            while (totalFastResume > 0)
            {
                var alerts = _session.Alerts.PopAll();

                foreach (var alert in alerts)
                {
                    var saveAlert = alert as SaveResumeDataAlert;
                    if (saveAlert == null)
                    {
                        continue;
                    }

                    totalFastResume -= 1;

                    if (saveAlert.ResumeData == null)
                    {
                        continue;
                    }

                    using (var handle = saveAlert.Handle)
                    using (var status = handle.QueryStatus())
                    {
                        var resumePath = _environment.GetApplicationDataPath()
                            .Combine("Torrents")
                            .CombineWithFilePath(string.Concat(status.InfoHash.ToHex(), ".resume"));

                        var file = _fileSystem.GetFile(resumePath);

                        using (var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            fileStream.Write(saveAlert.ResumeData, 0, saveAlert.ResumeData.Length);
                        }

                        _logger.Debug(string.Format("Resume-file {0} saved", resumePath.FullPath));
                    }
                }
            }

            var sessionStatePath = _environment.GetApplicationDataPath().CombineWithFilePath("session_state");
            var stateFile = _fileSystem.GetFile(sessionStatePath);
            var state = _session.SaveState();

            using (var stateStream = stateFile.OpenWrite())
            {
                stateStream.Write(state, 0, state.Length);
            }

            // Dispose session
            var sessImpl = _session as Session;
            if (sessImpl != null) sessImpl.Dispose();
        }

        public IEnumerable<ITorrent> GetAll()
        {
            return _session.GetTorrents().Select(Torrent.CreateFromHandle);
        }

        public ITorrent GetByInfoHash(string infoHash)
        {
            return null;
        }

        private void ReadAlerts()
        {
            var timeout = TimeSpan.FromSeconds(1);

            while (_alertsThreadRunning)
            {
                var foundAlerts = _session.Alerts.PeekWait(timeout);
                if (!foundAlerts) continue;

                var alerts = _session.Alerts.PopAll();

                foreach (var alert in alerts)
                {
                    _logger.Debug(alert.Message);

                    if (alert is TorrentAddedAlert)
                    {
                        Handle((TorrentAddedAlert) alert);
                    }
                }
            }
        }

        private void Handle(TorrentAddedAlert alert)
        {
            var torrent = Torrent.CreateFromHandle(alert.Handle);
            _messageBus.Publish(new TorrentAddedMessage(torrent));
        }

        private void AddTorrent(AddTorrentMessage message)
        {
            _logger.Debug("Adding torrent.");

            using (var addParams = new AddTorrentParams())
            {
                addParams.SavePath = "C:\\Downloads";
                addParams.TorrentInfo = new TorrentInfo(message.Data);

                // Save metadata file
                SaveMetadataFile(addParams.TorrentInfo);

                _session.AsyncAddTorrent(addParams);
            }
        }

        private void PauseTorrent(PauseTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;

                handle.AutoManaged = false;
                handle.Pause();
            }
        }

        private void ResumeTorrent(ResumeTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;

                handle.Resume();
                handle.AutoManaged = true;
            }
        }

        private void RemoveTorrent(RemoveTorrentMessage message)
        {
            using (var handle = _session.FindTorrent(message.InfoHash))
            {
                if (handle == null) return;
                _session.RemoveTorrent(handle);
            }
        }

        private void SaveMetadataFile(TorrentInfo info)
        {
            var torrentsPath = _environment.GetApplicationDataPath().Combine("Torrents");

            using (var creator = new TorrentCreator(info))
            {
                var data = creator.Generate();
                var filePath = torrentsPath.CombineWithFilePath(string.Format("{0}.torrent", info.InfoHash));
                var file = _fileSystem.GetFile(filePath);

                using (var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    fileStream.Write(data, 0, data.Length);
                }

                _logger.Debug("Saving torrent metadata to " + filePath.FullPath);
            }
        }
    }
}