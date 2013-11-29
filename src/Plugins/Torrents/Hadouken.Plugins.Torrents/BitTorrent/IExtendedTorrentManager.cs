using OctoTorrent.BEncoding;
using OctoTorrent.Client;

namespace Hadouken.Plugins.Torrents.BitTorrent
{
    public interface IExtendedTorrentManager
    {
        TorrentManager Manager { get; }
        string FriendlyInfoHash { get; }
        string Label { get; set; }

        long UploadedBytes { get; }
        long DownloadedBytes { get; }

        void Start();
        void Stop();
        void Pause();

        string GetMetadata(string key);
        void SetMetadata(string key, string value);
    }
}