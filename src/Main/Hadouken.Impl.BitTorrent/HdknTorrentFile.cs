using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.BitTorrent;
using MonoTorrent.Common;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknTorrentFile : ITorrentFile
    {
        private TorrentFile _tf;

        internal HdknTorrentFile(TorrentFile tf)
        {
            _tf = tf;
        }

        public IBitField BitField
        {
            get { return new HdknBitField(_tf.BitField); }
        }

        public long BytesDownloaded
        {
            get { return _tf.BytesDownloaded; }
        }

        public byte[] ED2K
        {
            get { return _tf.ED2K; }
        }

        public int EndPieceIndex
        {
            get { return _tf.EndPieceIndex; }
        }

        public long Length
        {
            get { return _tf.Length; }
        }

        public byte[] MD5
        {
            get { return _tf.MD5; }
        }

        public string Path
        {
            get { return _tf.Path; }
        }

        public Hadouken.BitTorrent.Priority Priority
        {
            get { return (Hadouken.BitTorrent.Priority)(int)_tf.Priority; }
            set { _tf.Priority = (MonoTorrent.Common.Priority)(int)value; }
        }

        public byte[] SHA1
        {
            get { return _tf.SHA1; }
        }

        public int StartPieceIndex
        {
            get { return _tf.StartPieceIndex; }
        }
    }
}
