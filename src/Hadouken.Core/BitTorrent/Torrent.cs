using Hadouken.Common.BitTorrent;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class Torrent : ITorrent
    {
        public string InfoHash { get; private set; }
        
        public string Name { get; private set; }
        
        public long Size { get; private set; }
        
        public float Progress { get; private set; }

        public ITorrentFile[] Files { get; private set; }

        internal static ITorrent CreateFromHandle(TorrentHandle handle)
        {
            using (handle)
            using (var file = handle.TorrentFile)
            using (var status = handle.QueryStatus())
            {
                var t = new Torrent
                {
                    InfoHash = handle.InfoHash.ToHex(),
                    Name = file.Name,
                    Size = file.TotalSize,
                    Progress = status.Progress
                };

                return t;
            }
        }
    }
}
