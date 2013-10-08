using MonoTorrent.Client;
using MonoTorrent.Common;

namespace Hadouken.Plugins.Torrents.Dto
{
    public class TorrentDetails : TorrentOverview
    {
        private readonly TorrentManager _manager;

        public TorrentDetails(TorrentManager manager): base(manager)
        {
            _manager = manager;
        }

        public TorrentFile[] Files
        {
            get { return _manager.Torrent.Files; }
        }
    }
}
