using System;
using Hadouken.Common.Messaging;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class SaveTorrentMetadataFileMessage : IMessage
    {
        private readonly TorrentInfo _torrentInfo;

        public SaveTorrentMetadataFileMessage(TorrentInfo torrentInfo)
        {
            if (torrentInfo == null) throw new ArgumentNullException("torrentInfo");
            _torrentInfo = torrentInfo;
        }

        public TorrentInfo TorrentInfo
        {
            get { return _torrentInfo; }
        }
    }
}
