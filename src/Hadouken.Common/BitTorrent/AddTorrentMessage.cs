using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class AddTorrentMessage : IMessage
    {
        private readonly byte[] _data;

        public AddTorrentMessage(byte[] data)
        {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
        }

        public byte[] Data
        {
            get { return _data; }
        }

        public string SavePath { get; set; }

        public string Label { get; set; }
    }
}
