using System.Collections.Generic;

namespace Hadouken.Common.BitTorrent
{
    public interface ITorrentEngine
    {
        IEnumerable<ITorrent> GetAll();

        ITorrent GetByInfoHash(string infoHash);
    }
}
