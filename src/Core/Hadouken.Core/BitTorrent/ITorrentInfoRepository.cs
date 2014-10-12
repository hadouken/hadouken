using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    public interface ITorrentInfoRepository
    {
        void Save(TorrentInfo info);

        void Remove(string infoHash);
    }
}
