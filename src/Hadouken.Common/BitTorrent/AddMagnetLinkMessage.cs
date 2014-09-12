using System;
using Hadouken.Common.Messaging;

namespace Hadouken.Common.BitTorrent
{
    public sealed class AddMagnetLinkMessage : IMessage
    {
        private readonly string _magnetLink;

        public AddMagnetLinkMessage(string magnetLink)
        {
            if (magnetLink == null) throw new ArgumentNullException("magnetLink");
            _magnetLink = magnetLink;
        }

        public string MagnetLink
        {
            get { return _magnetLink; }
        }

        public string Name { get; set; }

        public string SavePath { get; set; }

        public string Label { get; set; }

        public string[] Trackers { get; set; }
    }
}
