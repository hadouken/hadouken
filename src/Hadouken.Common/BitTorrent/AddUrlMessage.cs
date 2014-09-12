using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class AddUrlMessage : IMessage
    {
        private readonly string _url;

        public AddUrlMessage(string url)
        {
            if (url == null) throw new ArgumentNullException("url");
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }

        public string Name { get; set; }

        public string SavePath { get; set; }

        public string Label { get; set; }

        public string[] Trackers { get; set; }
    }
}
