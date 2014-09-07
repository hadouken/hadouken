using System.Net;

namespace Hadouken.Common.BitTorrent
{
    public interface IPeer
    {
        string EndPoint { get; }
    }
}
