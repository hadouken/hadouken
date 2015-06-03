using Hadouken.Common.BitTorrent;

namespace Hadouken.Core.BitTorrent {
    public sealed class TorrentSettings : ITorrentSettings {
        public int DownloadRateLimit { get; internal set; }
        public int MaxConnections { get; internal set; }
        public int MaxUploads { get; internal set; }
        public bool SequentialDownload { get; internal set; }
        public int UploadRateLimit { get; internal set; }
    }
}