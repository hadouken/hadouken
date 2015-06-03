using Hadouken.Common.BitTorrent;

namespace Hadouken.Core.BitTorrent {
    internal sealed class Peer : IPeer {
        public string Client { get; internal set; }
        public string Country { get; internal set; }
        public long DownloadSpeed { get; internal set; }
        public string Ip { get; internal set; }
        public float Progress { get; internal set; }
        public bool IsSeed { get; internal set; }
        public long UploadSpeed { get; internal set; }
    }
}