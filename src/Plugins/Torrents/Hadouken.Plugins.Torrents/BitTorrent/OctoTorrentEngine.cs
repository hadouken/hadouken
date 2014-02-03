using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.ExceptionServices;
using Hadouken.Framework;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Torrents.Dto;
using OctoTorrent;
using OctoTorrent.Client;
using OctoTorrent.Common;
using OctoTorrent.Dht;
using OctoTorrent.Dht.Listeners;
using System.Linq;

namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public sealed class OctoTorrentEngine : IBitTorrentEngine
    {
        private static readonly IDictionary<Tuple<TorrentState, TorrentState>, string> EventMap =
            new Dictionary<Tuple<TorrentState, TorrentState>, string>();

        private readonly IEngineSettingsFactory _settingsFactory;
        private readonly IJsonRpcClient _rpcClient;
        private readonly IBootConfig _config;
        private readonly object _managersLock = new object();
        private readonly IDictionary<string, IExtendedTorrentManager> _managers;
 
        private ClientEngine _engine;
        private DhtEngine _dhtEngine;

        static OctoTorrentEngine()
        {
            EventMap.Add(Tuple.Create(TorrentState.Downloading, TorrentState.Seeding), "torrent.completed");
            EventMap.Add(Tuple.Create(TorrentState.Stopped, TorrentState.Downloading), "torrent.started");
            EventMap.Add(Tuple.Create(TorrentState.Stopped, TorrentState.Seeding), "torrent.started");
            EventMap.Add(Tuple.Create(TorrentState.Hashing, TorrentState.Downloading), "torrent.started");
            EventMap.Add(Tuple.Create(TorrentState.Hashing, TorrentState.Seeding), "torrent.started");
        }

        public OctoTorrentEngine(IEngineSettingsFactory settingsFactory, IJsonRpcClient rpcClient, IBootConfig config)
        {
            _settingsFactory = settingsFactory;
            _rpcClient = rpcClient;
            _config = config;
            _managers = new Dictionary<string, IExtendedTorrentManager>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Load()
        {
            LoadEngine();
            LoadDht();
            LoadManagers();
        }

        private void LoadEngine()
        {
            var settings = _settingsFactory.Build();
            _engine = new ClientEngine(settings);
        }

        private void LoadDht()
        {
            var listenAddress = new IPEndPoint(IPAddress.Any, 56544);
            var listener = new DhtListener(listenAddress);

            _dhtEngine = new DhtEngine(listener);
            
            _engine.RegisterDht(_dhtEngine);
            listener.Start();
            _engine.DhtEngine.Start();
        }

        public void Unload()
        {
            UnloadManagers();
            _engine.Dispose();
        }

        private void UnloadManagers()
        {
            this.SaveManager(_managers.Values);

            foreach (var manager in Managers)
            {
                _engine.Unregister(manager.Manager);
            }
        }

        public IEnumerable<IExtendedTorrentManager> Managers
        {
            get { lock (_managersLock) return _managers.Values; }
        }

        public IExtendedTorrentManager Get(string infoHash)
        {
            lock (_managersLock)
            {
                if (_managers.ContainsKey(infoHash))
                    return _managers[infoHash];
            }

            return null;
        }

        public IExtendedTorrentManager Add(byte[] data, string savePath = null, 
            string label = null, bool propagateEvent = true, long uploadedBytes = 0, long downloadedBytes = 0)
        {
            Torrent torrent;

            if (!Torrent.TryLoad(data, out torrent))
                return null;
            if (String.IsNullOrEmpty(savePath))
                savePath = _engine.Settings.SavePath;

            var manager = new TorrentManager(torrent, savePath, new TorrentSettings());

            IExtendedTorrentManager extendedManager = new ExtendedTorrentManager(manager, uploadedBytes, downloadedBytes);
            extendedManager.Manager.TorrentStateChanged += Manager_TorrentStateChanged;

            if (this.Get(extendedManager.FriendlyInfoHash) != null)
                return null;

            this.SaveTorrent(extendedManager.FriendlyInfoHash, data);

            _engine.Register(manager);

            lock (_managersLock)
            {
                _managers.Add(extendedManager.FriendlyInfoHash, extendedManager);
            }

            if (propagateEvent)
            {
                _rpcClient.SendEventAsync("torrent.added", new TorrentOverview(extendedManager.Manager));
            }

            return extendedManager;
        }

        private void Manager_TorrentStateChanged(object sender, TorrentStateChangedEventArgs e)
        {
            var tuple = Tuple.Create(e.OldState, e.NewState);

            if (EventMap.ContainsKey(tuple))
            {
                var eventName = EventMap[tuple];
                _rpcClient.SendEventAsync(eventName, new TorrentOverview(e.TorrentManager));

                return;
            }

            switch (e.NewState)
            {
                case TorrentState.Error:
                    _rpcClient.SendEventAsync("torrent.error", new TorrentOverview(e.TorrentManager));
                    break;
                case TorrentState.Stopped:
                    _rpcClient.SendEventAsync("torrent.stopped", new TorrentOverview(e.TorrentManager));
                    break;
                case TorrentState.Paused:
                    _rpcClient.SendEventAsync("torrent.paused", new TorrentOverview(e.TorrentManager));
                    break;
            }
        }

        public void Remove(IExtendedTorrentManager manager)
        {
            throw new NotImplementedException();
        }

        private void LoadManagers()
        {
            var path = Path.Combine(_config.DataPath, "state.dat");
            if (!File.Exists(path))
            {
                return;
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
            using (var reader = new BinaryReader(stream))
            {
                var managerCount = reader.ReadInt32();
                for (int index = 0; index < managerCount; index++)
                {
                    var hash = reader.ReadString();
                    var label = (string)null;
                    var savePath = (string)null;

                    var haslabel = reader.ReadBoolean();
                    if (haslabel)
                    {
                        label = reader.ReadString();
                    }

                    var hasSavePath = reader.ReadBoolean();
                    if (hasSavePath)
                    {
                        savePath = reader.ReadString();
                    }                    

                    long uploadedByteCount = reader.ReadInt64();
                    long downloadedByteCount = reader.ReadInt64();

                    // Read the torrent data.
                    var data = this.LoadTorrent(hash);

                    // Add the manager.
                    this.Add(data, savePath, label, false, uploadedByteCount, downloadedByteCount);
                }
            }
        }

        private void SaveManager(IEnumerable<IExtendedTorrentManager> managers)
        {
            var managerList = new List<IExtendedTorrentManager>(managers);
            var path = Path.Combine(_config.DataPath, "state.dat");
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(managerList.Count);
                foreach (var manager in managerList)
                {
                    writer.Write(manager.FriendlyInfoHash);

                    writer.Write(manager.Label != null);
                    if (manager.Label != null)
                    {
                        writer.Write(manager.Label);
                    }

                    writer.Write(manager.Manager.SavePath != null);
                    if (manager.Manager.SavePath != null)
                    {
                        writer.Write(manager.Manager.SavePath);
                    }

                    writer.Write(manager.Manager.Monitor.DataBytesUploaded);
                    writer.Write(manager.Manager.Monitor.DataBytesDownloaded);
                }
            }
        }

        private void SaveTorrent(string hash, byte[] data)
        {
            var path = Path.Combine(_config.DataPath, string.Concat(hash, ".torrent"));
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(data.Length);
                writer.Write(data);
            }
        }

        private byte[] LoadTorrent(string hash)
        {
            // Read the torrent data.
            var torrentPath = Path.Combine(_config.DataPath, string.Concat(hash, ".torrent"));
            using (var torrentStream = new FileStream(torrentPath, FileMode.Open, FileAccess.Read, FileShare.None))
            using (var torrentReader = new BinaryReader(torrentStream))
            {
                // Read the torrent.
                var dataLength = torrentReader.ReadInt32();
                return torrentReader.ReadBytes(dataLength);
            }
        }
    }
}