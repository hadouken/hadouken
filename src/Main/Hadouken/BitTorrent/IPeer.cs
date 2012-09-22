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
        IPEndPoint EndPoint { get; }
        string ReverseDns { get; }
        string ClientSoftware { get; }
        int HashFails { get; }

        long DownloadedBytes { get; }
        long UploadedBytes { get; }
        long DownloadSpeed { get; }
        long UploadSpeed { get; }
        double Progress { get; }
    }
}
