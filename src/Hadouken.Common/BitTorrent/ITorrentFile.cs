namespace Hadouken.Common.BitTorrent
{
    public interface ITorrentFile
    {
        string Path { get; }

        long Size { get; }

        float Progress { get; }

        int Priority { get; }
    }
}
