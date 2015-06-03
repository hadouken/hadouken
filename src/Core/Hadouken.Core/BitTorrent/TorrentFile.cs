using Hadouken.Common.BitTorrent;

namespace Hadouken.Core.BitTorrent {
    internal sealed class TorrentFile : ITorrentFile {
        public TorrentFile(int index, string path, long size, long offset) {
            this.Index = index;
            this.Path = path;
            this.Size = size;
            this.Offset = offset;
        }

        public int Index { get; private set; }
        public string Path { get; private set; }
        public long Size { get; private set; }
        public long Offset { get; private set; }
    }
}