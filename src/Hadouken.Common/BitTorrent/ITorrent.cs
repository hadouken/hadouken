namespace Hadouken.Common.BitTorrent
{
    public interface ITorrent
    {
        string InfoHash { get; }

        string Name { get; }

        long Size { get; }

        float Progress { get; }

        ITorrentFile[] Files { get; }
    }
}
