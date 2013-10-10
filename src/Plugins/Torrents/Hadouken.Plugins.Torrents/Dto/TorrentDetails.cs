using System.Linq;
using OctoTorrent.Client;

namespace Hadouken.Plugins.Torrents.Dto
{
    public class TorrentDetails : TorrentOverview
    {
        private readonly TorrentManager _manager;

        public TorrentDetails(TorrentManager manager): base(manager)
        {
            _manager = manager;
        }

        public TorrentFileDetails[] Files
        {
            get { return _manager.Torrent.Files.Select((file, index) => new TorrentFileDetails(index, file)).ToArray(); }
        }
    }
}
