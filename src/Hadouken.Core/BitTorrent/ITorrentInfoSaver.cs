using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    public interface ITorrentInfoSaver
    {
        void Save(TorrentInfo info);
    }
}
