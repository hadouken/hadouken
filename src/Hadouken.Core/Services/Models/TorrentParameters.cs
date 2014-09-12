namespace Hadouken.Core.Services.Models
{
    public sealed class TorrentParameters
    {
        public int DownloadLimit { get; set; }

        public string Label { get; set; }

        public int MaxConnections { get; set; }

        public int MaxUploads { get; set; }

        public string Name { get; set; }

        public string SavePath { get; set; }

        public string[] Trackers { get; set; }

        public int UploadLimit { get; set; }
    }
}
