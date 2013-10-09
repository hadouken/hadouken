using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Torrents.Dto;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using System.IO;

namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public class MonoTorrentEngine : IBitTorrentEngine
    {
        private readonly string _dataPath;
        private readonly string _torrentsPath;
        private readonly JsonRpcClient _rpcClient;
        private readonly IEngineSettingsFactory _settingsFactory;
        private ClientEngine _engine;

        private readonly IDictionary<string, TorrentManager> _torrentManagers =
            new Dictionary<string, TorrentManager>(StringComparer.InvariantCultureIgnoreCase);

        public MonoTorrentEngine(string dataPath, Uri rpcUrl)
        {
            _dataPath = dataPath;
            _torrentsPath = Path.Combine(dataPath, "Torrents");
            _rpcClient = new JsonRpcClient(rpcUrl);
            _settingsFactory = new EngineSettingsFactory(rpcUrl);
        }

        public void Load()
        {
            var settings = _settingsFactory.Build();
            _engine = new ClientEngine(settings);
        }

        public void Unload()
        {
            _engine.Dispose();
        }

        public IEnumerable<TorrentManager> TorrentManagers
        {
            get { return new List<TorrentManager>(_torrentManagers.Values); }
        }

        public TorrentManager Get(string infoHash)
        {
            if (_torrentManagers.ContainsKey(infoHash))
                return _torrentManagers[infoHash];

            return null;
        }

        public TorrentManager Add(byte[] data, string savePath = null, string label = null)
        {
            Torrent torrent;

            if (!Torrent.TryLoad(data, out torrent))
                return null;

            try
            {
                SaveTorrentFile(data, torrent.Name + ".torrent");
            }
            catch (Exception)
            {
                // TODO: Log exception here
                return null;
            }

            if (String.IsNullOrEmpty(savePath))
                savePath = _engine.Settings.SavePath;

            var manager = new TorrentManager(torrent, savePath, new TorrentSettings());
            manager.TorrentStateChanged += TorrentStateChanged;

            _torrentManagers.Add(manager.InfoHash.ToString().Replace("-", ""), manager);
            _engine.Register(manager);

            SendEvent("torrent.added", new TorrentOverview(manager));

            return manager;
        }

        private void TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            if (e.OldState == TorrentState.Downloading
                && e.NewState == TorrentState.Seeding)
            {
                SendEvent("torrent.completed", new TorrentOverview(e.TorrentManager));
            }
            else if (e.OldState == TorrentState.Stopped
                     && e.NewState == TorrentState.Downloading)
            {
                SendEvent("torrent.started", new TorrentOverview(e.TorrentManager));
            }
            else if (e.OldState == TorrentState.Stopped
                     && e.NewState == TorrentState.Seeding)
            {
                SendEvent("torrent.started", new TorrentOverview(e.TorrentManager));
            }
            else if (e.OldState == TorrentState.Hashing
                     && e.NewState == TorrentState.Downloading)
            {
                SendEvent("torrent.started", new TorrentOverview(e.TorrentManager));
            }
            else if (e.OldState == TorrentState.Hashing
                     && e.NewState == TorrentState.Seeding)
            {
                SendEvent("torrent.started", new TorrentOverview(e.TorrentManager));
            }
            else if (e.NewState == TorrentState.Paused)
            {
                SendEvent("torrent.paused", new TorrentOverview(e.TorrentManager));
            }
            else if (e.NewState == TorrentState.Stopped)
            {
                SendEvent("torrent.stopped", new TorrentOverview(e.TorrentManager));
            }
            else if (e.NewState == TorrentState.Error)
            {
                SendEvent("torrent.error", new TorrentOverview(e.TorrentManager));
            }
        }

        private void SendEvent(string eventName, object data)
        {
            _rpcClient.CallAsync<bool>("events.publish", new object[]
            {
                eventName,
                data
            });
        }

        private void SaveTorrentFile(byte[] data, string fileName)
        {
            if (!Directory.Exists(_torrentsPath))
                Directory.CreateDirectory(_torrentsPath);

            // Save torrent to data path
            string torrentFile = Path.Combine(_dataPath, "Torrents", fileName);

            if (!File.Exists(torrentFile))
                File.WriteAllBytes(torrentFile, data);
        }

        public void Delete(string infoHash)
        {
            throw new System.NotImplementedException();
        }

        public void Move(string infoHash, string targetPath)
        {
            throw new System.NotImplementedException();
        }
    }
}