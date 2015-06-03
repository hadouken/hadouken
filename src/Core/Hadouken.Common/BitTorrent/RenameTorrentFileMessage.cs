using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class RenameTorrentFileMessage : IMessage {
        private readonly int _fileIndex;
        private readonly string _fileName;
        private readonly string _infoHash;

        public RenameTorrentFileMessage(string infoHash, int fileIndex, string fileName) {
            if (infoHash == null) {
                throw new ArgumentNullException("infoHash");
            }
            if (fileName == null) {
                throw new ArgumentNullException("fileName");
            }
            this._infoHash = infoHash;
            this._fileIndex = fileIndex;
            this._fileName = fileName;
        }

        public string InfoHash {
            get { return this._infoHash; }
        }

        public int FileIndex {
            get { return this._fileIndex; }
        }

        public string FileName {
            get { return this._fileName; }
        }
    }
}