using Hadouken.Common.BitTorrent;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class TorrentFile : ITorrentFile
    {
        public string Path { get; private set; }

        public long Size { get; private set; }
        
        public float Progress { get; private set; }
        
        public int Priority { get; private set; }

        public static ITorrentFile CreateFromEntry(FileEntry entry, long progress, int priority)
        {
            using (entry)
            {
                return new TorrentFile
                {
                    Path = entry.Path,
                    Priority = priority,
                    Progress = (progress / (float) entry.Size) * 100f,
                    Size = entry.Size
                };
            }
        }
    }
}
