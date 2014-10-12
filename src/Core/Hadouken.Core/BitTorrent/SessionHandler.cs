using System;
using System.Collections.Generic;
using System.IO;
using Hadouken.Common;
using Hadouken.Common.Data;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Common.Messaging;
using Hadouken.Common.Timers;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal class SessionHandler : ISessionHandler
    {
        private readonly ILogger<SessionHandler> _logger;
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IMessageBus _messageBus;
        private readonly ISession _session;
        private readonly IAlertBus _alertBus;
        private readonly ITorrentManager _torrentManager;
        private readonly IList<string> _muted;
        private readonly ITimer _timer;

        public SessionHandler(ILogger<SessionHandler> logger,
            IEnvironment environment,
            IFileSystem fileSystem,
            IKeyValueStore keyValueStore,
            IMessageBus messageBus,
            ISession session,
            IAlertBus alertBus,
            ITorrentManager torrentManager,
            ITimerFactory timerFactory)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (messageBus == null) throw new ArgumentNullException("messageBus");
            if (session == null) throw new ArgumentNullException("session");
            if (alertBus == null) throw new ArgumentNullException("alertBus");
            if (torrentManager == null) throw new ArgumentNullException("torrentManager");

            _logger = logger;
            _environment = environment;
            _fileSystem = fileSystem;
            _keyValueStore = keyValueStore;
            _messageBus = messageBus;
            _session = session;
            _alertBus = alertBus;
            _torrentManager = torrentManager;
            _muted = new List<string>();
            _timer = timerFactory.Create(1000, PostTorrentUpdates);
        }

        public void Load()
        {
            // Set up message subscriptions
            _logger.Debug("Setting up subscriptions.");
            
            // Configure session
            _logger.Debug("Setting alert mask.");
            _session.SetAlertMask(SessionAlertCategory.Error
                                  | SessionAlertCategory.Peer
                                  | SessionAlertCategory.Stats
                                  | SessionAlertCategory.Status);

            var listenPort = _keyValueStore.Get<int>("bt.net.listen_port");
            _session.ListenOn(listenPort, listenPort);

            _logger.Debug("Session listening on port {Port}.", listenPort);

            // Reload session settings
            _messageBus.Publish(new KeyValueChangedMessage(new[] {"bt.forceReloadSettings"}));

            _torrentManager.Load();

            // Start alerts thread
            _alertBus.StartRead();

            // Load previous session state (if any)
            var sessionStatePath = _environment.GetApplicationDataPath().CombineWithFilePath("session_state");
            if (_fileSystem.Exist(sessionStatePath))
            {
                _logger.Debug("Loading session state.");

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

                    addParams.SavePath = _keyValueStore.Get<string>("bt.save_path");
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

                    // Add to muted torrents so we don't notify anyone
                    _muted.Add(addParams.TorrentInfo.InfoHash);

                    // Add torrent to session
                    _session.AsyncAddTorrent(addParams);
                }
            }

            _timer.Start();
        }

        public void Unload()
        {
            _timer.Stop();
            _alertBus.StopRead();

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

        private void PostTorrentUpdates()
        {
            _session.PostTorrentUpdates();
        }
    }
}