namespace Hadouken.Common.BitTorrent
{
    public interface ITorrentFile
    {
        int Index { get; }

        string Path { get; }

        long Size { get; }

        long Offset { get; }
    }
}
