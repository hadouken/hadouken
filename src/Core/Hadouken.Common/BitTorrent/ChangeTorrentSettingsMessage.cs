using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class ChangeTorrentSettingsMessage : IMessage {
        private readonly int _downloadRateLimit;
        private readonly string _infoHash;
        private readonly int _maxConnections;
        private readonly int _maxUploads;
        private readonly bool _sequentialDownload;
        private readonly int _uploadRateLimit;

        public ChangeTorrentSettingsMessage(string infoHash,
            int downloadRateLimit,
            int maxConnections,
            int maxUploads,
            bool sequentialDownload,
            int uploadRateLimit) {
            this._infoHash = infoHash;
            this._downloadRateLimit = downloadRateLimit;
            this._maxConnections = maxConnections;
            this._maxUploads = maxUploads;
            this._sequentialDownload = sequentialDownload;
            this._uploadRateLimit = uploadRateLimit;
        }

        public int DownloadRateLimit {
            get { return this._downloadRateLimit; }
        }

        public int MaxConnections {
            get { return this._maxConnections; }
        }

        public int MaxUploads {
            get { return this._maxUploads; }
        }

        public bool SequentialDownload {
            get { return this._sequentialDownload; }
        }

        public int UploadRateLimit {
            get { return this._uploadRateLimit; }
        }

        public string InfoHash {
            get { return this._infoHash; }
        }
    }
}