using System.Net;
using Hadouken.Common.BitTorrent;
using Ragnar;

namespace Hadouken.Core.BitTorrent
{
    internal sealed class Peer : IPeer
    {
        public string EndPoint { get; private set; }

        public static IPeer CreateFromPeerInfo(PeerInfo info)
        {
            using (info)
            {
                return new Peer
                {
                    EndPoint = info.EndPoint.ToString()
                };
            }
        }
    }
}
