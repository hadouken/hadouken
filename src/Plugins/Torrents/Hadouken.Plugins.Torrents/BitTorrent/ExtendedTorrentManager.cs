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
        private readonly long _initialUploadedBytes;
        private readonly long _initialDownloadedBytes;

        private readonly IDictionary<string, string> _metadata =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase); 

        public ExtendedTorrentManager(TorrentManager manager, long uploadedBytes = 0, long downloadedBytes = 0)
        {
            _manager = manager;
            _initialUploadedBytes = uploadedBytes;
            _initialDownloadedBytes = downloadedBytes;
        }

        public TorrentManager Manager
        {
            get { return _manager; }
        }

        public string FriendlyInfoHash
        {
            get { return _manager.InfoHash.ToString().Replace("-", "").ToLowerInvariant(); }
        }

        public long UploadedBytes
        {
            get { return _initialUploadedBytes + _manager.Monitor.DataBytesUploaded; }
        }

        public long DownloadedBytes
        {
            get { return _initialDownloadedBytes + _manager.Monitor.DataBytesDownloaded; }
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
    }
}
