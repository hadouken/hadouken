using System;
using System.Collections.Generic;
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
        private ClientEngine _engine;

        private readonly IDictionary<string, TorrentManager> _torrentManagers =
            new Dictionary<string, TorrentManager>(StringComparer.InvariantCultureIgnoreCase);

        public MonoTorrentEngine(string dataPath)
        {
            _dataPath = dataPath;
            _torrentsPath = Path.Combine(dataPath, "Torrents");
        }

        public void Load()
        {
            _engine = new ClientEngine(new EngineSettings
            {
                AllowedEncryption = EncryptionTypes.All,
                GlobalMaxConnections = 200,
                SavePath = @"C:\Temporary"
            });
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

            if (!Directory.Exists(_torrentsPath))
                Directory.CreateDirectory(_torrentsPath);

            // Save torrent to data path
            string torrentFile = Path.Combine(_dataPath, "Torrents", torrent.Name + ".torrent");

            if (!File.Exists(torrentFile))
                File.WriteAllBytes(torrentFile, data);

            if (String.IsNullOrEmpty(savePath))
                savePath = _engine.Settings.SavePath;

            var manager = new TorrentManager(torrent, savePath, new TorrentSettings());

            _torrentManagers.Add(manager.InfoHash.ToString(), manager);
            _engine.Register(manager);

            return manager;
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