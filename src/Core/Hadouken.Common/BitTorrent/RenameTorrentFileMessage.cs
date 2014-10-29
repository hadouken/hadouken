using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class RenameTorrentFileMessage : IMessage
    {
        private readonly string _infoHash;
        private readonly int _fileIndex;
        private readonly string _fileName;

        public RenameTorrentFileMessage(string infoHash, int fileIndex, string fileName)
        {
            if (infoHash == null) throw new ArgumentNullException("infoHash");
            if (fileName == null) throw new ArgumentNullException("fileName");
            _infoHash = infoHash;
            _fileIndex = fileIndex;
            _fileName = fileName;
        }

        public string InfoHash
        {
            get { return _infoHash; }
        }

        public int FileIndex
        {
            get { return _fileIndex; }
        }

        public string FileName
        {
            get { return _fileName; }
        }
    }
}
