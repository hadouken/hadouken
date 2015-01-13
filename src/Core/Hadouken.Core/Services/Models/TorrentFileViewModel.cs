namespace Hadouken.Core.Services.Models
{
    using Hadouken.Common.BitTorrent;
    using Hadouken.Common.IO;
    using Hadouken.Common.Text;

    public class TorrentFileViewModel
    {
        private readonly IStringEncoder _stringEncoder;

        public string Path { get; set; }

        public TorrentFileViewModel(ITorrentFile torrentFile)
        {
            _stringEncoder = new Windows1251StringEncoder();
            Path = _stringEncoder.Encode(torrentFile.Path);
        }
    }
}