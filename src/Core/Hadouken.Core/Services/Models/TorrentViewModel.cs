namespace Hadouken.Core.Services.Models
{
    using Hadouken.Common.BitTorrent;
    using Hadouken.Common.Text;
    
    public class TorrentViewModel
    {
        private readonly IStringEncoder _stringEncoder;

        public TorrentViewModel(ITorrent torrent)
        {
            _stringEncoder = new Windows1251StringEncoder();

            DownloadSpeed = torrent.DownloadSpeed;
            Error = torrent.Error;
            Name = _stringEncoder.Encode(torrent.Name);
            Paused = torrent.Paused;
            Progress = torrent.Progress;
            QueuePosition = torrent.QueuePosition;
            SavePath = torrent.SavePath;
            Size = torrent.Size;
            State = torrent.State;
            TotalDownloadedBytes = torrent.TotalDownloadedBytes;
            TotalRemainingBytes = torrent.TotalRemainingBytes;
            TotalUploadedBytes = torrent.TotalUploadedBytes;
            UploadSpeed = torrent.UploadSpeed;
            InfoHash = torrent.InfoHash;
        }

        public string InfoHash { get; set; }

        public long UploadSpeed { get; set; }

        public long TotalUploadedBytes { get; set; }

        public long TotalRemainingBytes { get; set; }

        public long TotalDownloadedBytes { get; set; }

        public TorrentState State { get; set; }

        public long Size { get; set; }

        public string SavePath { get; set; }

        public int QueuePosition { get; set; }

        public float Progress { get; set; }

        public bool Paused { get; set; }

        public string Name { get; set; }

        public string Error { get; set; }

        public long DownloadSpeed { get; set; }
    }
}