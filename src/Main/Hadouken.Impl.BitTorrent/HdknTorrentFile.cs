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

        public string Name
        {
            get { return _tf.FullPath; }
        }

        public long Size
        {
            get { return _tf.Length; }
        }
    }
}
