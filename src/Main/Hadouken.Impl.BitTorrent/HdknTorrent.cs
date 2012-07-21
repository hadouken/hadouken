using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using MonoTorrent.Client;
using MonoTorrent.Common;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTorrent : ITorrent
    {
        private Torrent _torrent;
        private List<HdknTorrentFile> _files = new List<HdknTorrentFile>();

        private byte[] _data;

        internal HdknTorrent(Torrent torrent)
        {
            _torrent = torrent;

            foreach (var file in torrent.Files)
            {
                _files.Add(new HdknTorrentFile(file));
            }
        }

        public List<List<string>> AnnounceUrls
        {
            get { return _torrent.AnnounceUrls; }
        }

        public string Comment
        {
            get { return _torrent.Comment; }
        }

        public string CreatedBy
        {
            get { return _torrent.CreatedBy; }
        }

        public DateTime CreationDate
        {
            get { return _torrent.CreationDate; }
        }

        public byte[] ED2K
        {
            get { return _torrent.ED2K; }
        }

        public string Encoding
        {
            get { return _torrent.Encoding; }
        }

        public List<string> GetRightHttpSeeds
        {
            get { return _torrent.GetRightHttpSeeds; }
        }

        public string InfoHash
        {
            get { return _torrent.InfoHash.ToString().Replace("-", ""); }
        }

        public bool IsPrivate
        {
            get { return _torrent.IsPrivate; }
        }

        public string Name
        {
            get { return _torrent.Name; }
        }

        public int PieceLength
        {
            get { return _torrent.PieceLength; }
        }

        public string Publisher
        {
            get { return _torrent.Publisher; }
        }

        public string PublisherUrl
        {
            get { return _torrent.PublisherUrl; }
        }

        public byte[] SHA1
        {
            get { return _torrent.SHA1; }
        }

        public long Size
        {
            get { return _torrent.Size; }
        }

        public string Source
        {
            get { return _torrent.Source; }
        }

        public ITorrentFile[] Files
        {
            get { return _files.ToArray<ITorrentFile>(); }
        }
    }
}
