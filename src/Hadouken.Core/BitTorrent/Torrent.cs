using System.Linq;
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

        public string SavePath { get; private set; }

        public long DownloadSpeed { get; private set; }
        
        public long UploadSpeed { get; private set; }
        
        public long TotalDownloadedBytes { get; private set; }
        
        public long TotalUploadedBytes { get; private set; }

        public Common.BitTorrent.TorrentState State { get; private set; }

        public bool Paused { get; private set; }

        public ITorrentFile[] Files { get; private set; }

        public IPeer[] Peers { get; private set; }

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
                    SavePath = status.SavePath,
                    Size = file.TotalSize,
                    Progress = status.Progress,
                    DownloadSpeed = status.DownloadRate,
                    UploadSpeed = status.UploadRate,
                    TotalDownloadedBytes = status.TotalDownload,
                    TotalUploadedBytes = status.TotalUpload,
                    State = (Common.BitTorrent.TorrentState) (int) status.State,
                    Paused = status.Paused,
                    Files = new ITorrentFile[file.NumFiles],
                    Peers = handle.GetPeerInfo().Select(Peer.CreateFromPeerInfo).ToArray()
                };

                var progresses = handle.GetFileProgresses();
                var priorities = handle.GetFilePriorities();

                for (var i = 0; i < file.NumFiles; i++)
                {
                    var entry = file.FileAt(i);
                    var torrentFile = TorrentFile.CreateFromEntry(entry, progresses[i], priorities[i]);

                    t.Files[i] = torrentFile;
                }

                return t;
            }
        }
    }
}
