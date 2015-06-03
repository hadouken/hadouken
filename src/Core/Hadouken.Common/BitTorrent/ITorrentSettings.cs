namespace Hadouken.Common.BitTorrent {
    public interface ITorrentSettings {
        int DownloadRateLimit { get; }
        int MaxConnections { get; }
        int MaxUploads { get; }
        bool SequentialDownload { get; }
        int UploadRateLimit { get; }
    }
}