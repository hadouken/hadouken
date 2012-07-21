using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.BitTorrent
{
    public interface IPeer
    {
        string PeerId { get; }
        bool IsSeeder { get; }
        IPEndPoint Endpoint { get; }
    }
}
