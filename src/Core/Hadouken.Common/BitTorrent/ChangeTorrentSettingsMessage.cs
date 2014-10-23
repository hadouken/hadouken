using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class ChangeTorrentSettingsMessage : IMessage
    {
        private readonly string _infoHash;
        private readonly int _downloadRateLimit;
        private readonly int _maxConnections;
        private readonly int _maxUploads;
        private readonly bool _sequentialDownload;
        private readonly int _uploadRateLimit;

        public ChangeTorrentSettingsMessage(string infoHash,
            int downloadRateLimit,
            int maxConnections,
            int maxUploads,
            bool sequentialDownload,
            int uploadRateLimit)
        {
            _infoHash = infoHash;
            _downloadRateLimit = downloadRateLimit;
            _maxConnections = maxConnections;
            _maxUploads = maxUploads;
            _sequentialDownload = sequentialDownload;
            _uploadRateLimit = uploadRateLimit;
        }

        public int DownloadRateLimit
        {
            get { return _downloadRateLimit; }
        }

        public int MaxConnections
        {
            get { return _maxConnections; }
        }

        public int MaxUploads
        {
            get { return _maxUploads; }
        }

        public bool SequentialDownload
        {
            get { return _sequentialDownload; }
        }

        public int UploadRateLimit
        {
            get { return _uploadRateLimit; }
        }

        public string InfoHash
        {
            get { return _infoHash; }
        }
    }
}
