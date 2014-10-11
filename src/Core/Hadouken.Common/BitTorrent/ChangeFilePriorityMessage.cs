using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class ChangeFilePriorityMessage : IMessage
    {
        private readonly string _infoHash;
        private readonly int _fileIndex;
        private readonly int _priority;

        public ChangeFilePriorityMessage(string infoHash, int fileIndex, int priority)
        {
            if (infoHash == null) throw new ArgumentNullException("infoHash");
            _infoHash = infoHash;
            _fileIndex = fileIndex;
            _priority = priority;
        }

        public string InfoHash
        {
            get { return _infoHash; }
        }

        public int FileIndex
        {
            get { return _fileIndex; }
        }

        public int Priority
        {
            get { return _priority; }
        }
    }
}
