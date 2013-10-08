using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoTorrent.Client;

namespace Hadouken.Plugins.Torrents.Rpc.Dto
{
    public class TorrentDetails : TorrentOverview
    {
        private readonly TorrentManager _manager;

        public TorrentDetails(TorrentManager manager): base(manager)
        {
            _manager = manager;
        }
    }
}
