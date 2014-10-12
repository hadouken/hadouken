namespace Hadouken.Common.BitTorrent
{
    public interface IPeer
    {
        string Client { get; }

        string Country { get; }

        long DownloadSpeed { get; }

        string IP { get; }

        float Progress { get; }

        bool IsSeed { get; }

        long UploadSpeed { get; }
    }
}
