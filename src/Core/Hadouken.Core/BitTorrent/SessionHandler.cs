using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Common;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Hadouken.Common.Timers;
using Ragnar;

namespace Hadouken.Core.BitTorrent {
    internal class SessionHandler : ISessionHandler {
        private const int SaveInterval = 300;
        private readonly IAlertBus _alertBus;
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IKeyValueStore _keyValueStore;
        private readonly ILogger<SessionHandler> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IList<string> _muted;
        private readonly ISession _session;
        private readonly ITimer _timer;
        private readonly ITorrentManager _torrentManager;

        public SessionHandler(ILogger<SessionHandler> logger,
            IEnvironment environment,
            IFileSystem fileSystem,
            IKeyValueStore keyValueStore,
            IMessageBus messageBus,
            ISession session,
            IAlertBus alertBus,
            ITorrentManager torrentManager,
            ITimerFactory timerFactory) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (environment == null) {
                throw new ArgumentNullException("environment");
            }
            if (fileSystem == null) {
                throw new ArgumentNullException("fileSystem");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (messageBus == null) {
                throw new ArgumentNullException("messageBus");
            }
            if (session == null) {
                throw new ArgumentNullException("session");
            }
            if (alertBus == null) {
                throw new ArgumentNullException("alertBus");
            }
            if (torrentManager == null) {
                throw new ArgumentNullException("torrentManager");
            }

            this._logger = logger;
            this._environment = environment;
            this._fileSystem = fileSystem;
            this._keyValueStore = keyValueStore;
            this._messageBus = messageBus;
            this._session = session;
            this._alertBus = alertBus;
            this._torrentManager = torrentManager;
            this._muted = new List<string>();
            this._timer = timerFactory.Create(1000, this.Tick);
        }

        public void Load() {
            // Set up message subscriptions
            this._logger.Debug("Setting up subscriptions.");

            // Configure session
            this._logger.Debug("Setting alert mask.");
            this._session.SetAlertMask(SessionAlertCategory.Error
                                       | SessionAlertCategory.Peer
                                       | SessionAlertCategory.Stats
                                       | SessionAlertCategory.Status);

            var listenPort = this._keyValueStore.Get<int>("bt.net.listen_port");
            this._session.ListenOn(listenPort, listenPort);

            this._logger.Debug("Session listening on port {Port}.", listenPort);

            // Reload session settings
            this._messageBus.Publish(new KeyValueChangedMessage(new[] {"bt.forceReloadSettings"}));

            this._torrentManager.Load();

            // Start alerts thread
            this._alertBus.StartRead();

            // Load previous session state (if any)
            var sessionStatePath = this._environment.GetApplicationDataPath().CombineWithFilePath("session_state");
            if (this._fileSystem.Exist(sessionStatePath)) {
                this._logger.Debug("Loading session state.");

                var file = this._fileSystem.GetFile(sessionStatePath);

                using (var stateStream = file.OpenRead()) {
                    using (var ms = new MemoryStream()) {
                        stateStream.CopyTo(ms);
                        this._session.LoadState(ms.ToArray());

                        this._logger.Debug("Loaded session state.");
                    }
                }
            }

            // Load previous torrents (if any)
            var torrentsPath = this._environment.GetApplicationDataPath().Combine("Torrents");
            var torrentsDirectory = this._fileSystem.GetDirectory(torrentsPath);

            if (!torrentsDirectory.Exists) {
                torrentsDirectory.Create();
            }

            foreach (var file in torrentsDirectory.GetFiles("*.torrent", SearchScope.Current)) {
                using (var torrentStream = file.OpenRead()) {
                    using (var torrentMemoryStream = new MemoryStream()) {
                        using (var addParams = new AddTorrentParams()) {
                            torrentStream.CopyTo(torrentMemoryStream);

                            addParams.SavePath = this._keyValueStore.Get<string>("bt.save_path");
                            addParams.TorrentInfo = new TorrentInfo(torrentMemoryStream.ToArray());

                            this._logger.Debug("Loading " + addParams.TorrentInfo.Name);

                            // Check resume data
                            var resumePath = file.Path.ChangeExtension(".resume");
                            var resumeFile = this._fileSystem.GetFile(resumePath);

                            if (resumeFile.Exists) {
                                this._logger.Debug("Loading resume data for " + addParams.TorrentInfo.Name);

                                using (var resumeStream = resumeFile.OpenRead()) {
                                    using (var resumeMemoryStream = new MemoryStream()) {
                                        resumeStream.CopyTo(resumeMemoryStream);
                                        addParams.ResumeData = resumeMemoryStream.ToArray();
                                    }
                                }
                            }

                            // Add to muted torrents so we don't notify anyone
                            this._muted.Add(addParams.TorrentInfo.InfoHash);

                            // Add torrent to session
                            this._session.AsyncAddTorrent(addParams);
                        }
                    }
                }
            }

            this._timer.Start();
        }

        public void Unload() {
            this._timer.Stop();
            this._alertBus.StopRead();

            // Save state
            this._session.Pause();

            var totalFastResume = 0;

            foreach (var handle in this._session.GetTorrents()) {
                using (handle) {
                    using (var status = handle.QueryStatus()) {
                        if (!status.HasMetadata) {
                            this._logger.Debug(string.Format("Skipping {0}, no metadata", status.Name));
                            continue;
                        }

                        if (!status.NeedSaveResume) {
                            this._logger.Debug(string.Format("Skipping {0}, resume-file up to date", status.Name));
                            continue;
                        }

                        handle.SaveResumeData();
                        totalFastResume += 1;
                    }
                }
            }

            this._logger.Debug(string.Format("{0} torrents needs to save resume-file", totalFastResume));

            while (totalFastResume > 0) {
                var alerts = this._session.Alerts.PopAll();

                foreach (var saveAlert in alerts.OfType<SaveResumeDataAlert>()) {
                    totalFastResume -= 1;

                    if (saveAlert.ResumeData == null) {
                        continue;
                    }

                    using (var handle = saveAlert.Handle) {
                        using (var status = handle.QueryStatus()) {
                            var resumePath = this._environment.GetApplicationDataPath()
                                .Combine("Torrents")
                                .CombineWithFilePath(string.Concat(status.InfoHash.ToHex(), ".resume"));

                            var file = this._fileSystem.GetFile(resumePath);

                            using (var fileStream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None)) {
                                fileStream.Write(saveAlert.ResumeData, 0, saveAlert.ResumeData.Length);
                            }

                            this._logger.Debug(string.Format("Resume-file {0} saved", resumePath.FullPath));
                        }
                    }
                }
            }

            var sessionStatePath = this._environment.GetApplicationDataPath().CombineWithFilePath("session_state");
            var stateFile = this._fileSystem.GetFile(sessionStatePath);
            var state = this._session.SaveState();

            using (var stateStream = stateFile.OpenWrite()) {
                stateStream.Write(state, 0, state.Length);
            }

            // Dispose session
            var sessImpl = this._session as Session;
            if (sessImpl != null) {
                sessImpl.Dispose();
            }
        }

        private void Tick() {
            if (this._timer.Ticks%SaveInterval == 0) {
                this._logger.Trace("Saving resume data for torrents.");

                foreach (var torrent in this._torrentManager.Torrents.Values.Where(torrent => torrent.Handle.NeedSaveResumeData())) {
                    torrent.Handle.SaveResumeData();
                }
            }

            this._session.PostTorrentUpdates();
        }
    }
}