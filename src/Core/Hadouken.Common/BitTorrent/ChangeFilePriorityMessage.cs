using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent {
    public sealed class ChangeFilePriorityMessage : IMessage {
        private readonly int _fileIndex;
        private readonly string _infoHash;
        private readonly int _priority;

        public ChangeFilePriorityMessage(string infoHash, int fileIndex, int priority) {
            if (infoHash == null) {
                throw new ArgumentNullException("infoHash");
            }
            this._infoHash = infoHash;
            this._fileIndex = fileIndex;
            this._priority = priority;
        }

        public string InfoHash {
            get { return this._infoHash; }
        }

        public int FileIndex {
            get { return this._fileIndex; }
        }

        public int Priority {
            get { return this._priority; }
        }
    }
}