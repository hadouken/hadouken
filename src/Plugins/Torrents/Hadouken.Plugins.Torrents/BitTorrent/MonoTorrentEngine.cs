using System;
using System.Collections.Generic;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;

namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public class MonoTorrentEngine : IBitTorrentEngine
    {
        private ClientEngine _engine;

        private readonly IDictionary<string, TorrentManager> _torrentManagers =
            new Dictionary<string, TorrentManager>(StringComparer.InvariantCultureIgnoreCase); 

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