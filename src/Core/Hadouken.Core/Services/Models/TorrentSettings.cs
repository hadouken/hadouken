namespace Hadouken.Core.Services.Models
{
    public sealed class TorrentSettings
    {
        public int DownloadRateLimit { get; set; }

        public int MaxConnections { get; set; }

        public int MaxUploads { get; set; }

        public bool SequentialDownload { get; set; }

        public int UploadRateLimit { get; set; }
    }
}
