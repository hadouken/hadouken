using MonoTorrent.Common;
using System.Linq;

namespace Hadouken.Plugins.Torrents.Dto
{
    public class TorrentFileDetails
    {
        private readonly TorrentFile _torrentFile;

        public TorrentFileDetails(int index, TorrentFile torrentFile)
        {
            Index = index;
            _torrentFile = torrentFile;
        }

        public int Index { get; private set; }

        public int[] BitField {
            get { return _torrentFile.BitField.Select(b => b ? 1 : 0).ToArray(); }
        }

        public double BitFieldPercentComplete
        {
            get { return _torrentFile.BitField.PercentComplete; }
        }

        public long BytesDownloaded {get { return _torrentFile.BytesDownloaded; } }

        public int EndPieceIndex
        {
            get { return _torrentFile.EndPieceIndex; }
        }

        public string FullPath
        {
            get { return _torrentFile.FullPath; }
        }

        public long Length
        {
            get { return _torrentFile.Length; }
        }

        public byte[] MD5
        {
            get { return _torrentFile.MD5; }
        }

        public string Path
        {
            get { return _torrentFile.Path; }
        }

        public Priority Priority
        {
            get { return _torrentFile.Priority; }
        }

        public byte[] SHA1
        {
            get { return _torrentFile.SHA1; }
        }

        public int StartPieceIndex
        {
            get { return _torrentFile.StartPieceIndex; }
        }
    }
}
