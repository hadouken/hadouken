using MonoTorrent.Client;

namespace Hadouken.Plugins.Torrents.Dto
{
    public class TorrentOverview
    {
        private readonly TorrentManager _manager;

        public TorrentOverview(TorrentManager manager)
        {
            _manager = manager;
        }

        public string Id
        {
            get { return _manager.InfoHash.ToString().Replace("-", "").ToLowerInvariant(); }
        }

        public string Name
        {
            get { return _manager.Torrent.Name; }
        }

        public long Size
        {
            get { return _manager.Torrent.Size; }
        }

        public string State
        {
            get { return _manager.State.ToString(); }
        }

        public double Progress
        {
            get { return _manager.Progress; }
        }

        public string SavePath
        {
            get { return _manager.SavePath; }
        }
    }
}
