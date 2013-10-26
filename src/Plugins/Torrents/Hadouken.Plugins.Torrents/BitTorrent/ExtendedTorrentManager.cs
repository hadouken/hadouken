using System;
using System.Collections.Generic;
using OctoTorrent.BEncoding;
using OctoTorrent.Client;

namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public class ExtendedTorrentManager : IExtendedTorrentManager
    {
        private readonly TorrentManager _manager;
        private readonly object _metaLock = new object();

        private readonly IDictionary<string, string> _metadata =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase); 

        public ExtendedTorrentManager(TorrentManager manager)
        {
            _manager = manager;
        }

        public TorrentManager Manager
        {
            get { return _manager; }
        }

        public string FriendlyInfoHash
        {
            get { return _manager.InfoHash.ToString().Replace("-", "").ToLowerInvariant(); }
        }

        public string Label
        {
            get { return GetMetadata("label"); }
            set { SetMetadata("label", value); }
        }

        public void Start()
        {
            _manager.Start();
        }

        public void Stop()
        {
            _manager.Stop();
        }

        public void Pause()
        {
            _manager.Pause();
        }

        public string GetMetadata(string key)
        {
            lock (_metaLock)
            {
                if (_metadata.ContainsKey(key))
                    return _metadata[key];
            }

            return null;
        }

        public void SetMetadata(string key, string value)
        {
            lock (_metaLock)
            {
                _metadata[key] = value;
            }
        }

        public BEncodedDictionary Save()
        {
            return null;
        }

        public void Load(BEncodedDictionary dictionary)
        {
            
        }
    }
}
