using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.BitTorrent;
using MonoTorrent.Client;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknPeer : IPeer
    {
        private PeerId _peer;

        internal HdknPeer(PeerId peer)
        {
            _peer = peer;
        }

        public System.Net.IPEndPoint Endpoint
        {
            get { return null; }
        }
    }
}
