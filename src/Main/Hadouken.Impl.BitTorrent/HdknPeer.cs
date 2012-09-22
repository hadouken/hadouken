using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.BitTorrent;
using MonoTorrent.Client;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Hadouken.Impl.BitTorrent
{
    public class HdknPeer : IPeer
    {
        private PeerId _peer;
        private string _reverseDns;

        internal HdknPeer(PeerId peer)
        {
            _peer = peer;

            new Task(() =>
            {
                try
                {
                    _reverseDns = Dns.GetHostEntry(_peer.Uri.Host).HostName;
                }
                catch (SocketException) { }
            }).Start();
        }

        public string PeerId
        {
            get { return _peer.PeerID; }
        }

        public bool IsSeeder
        {
            get { return _peer.IsSeeder; }
        }

        public IPEndPoint EndPoint
        {
            get { return (IPEndPoint)_peer.Connection.EndPoint; }
        }

        public string ReverseDns
        {
            get { return _reverseDns; }
        }

        public string ClientSoftware
        {
            get { return _peer.ClientApp.Client.ToString(); }
        }

        public int HashFails
        {
            get { return _peer.HashFails; }
        }

        public long DownloadedBytes
        {
            get { return _peer.Monitor.DataBytesDownloaded; }
        }

        public long UploadedBytes
        {
            get { return _peer.Monitor.DataBytesUploaded; }
        }

        public long DownloadSpeed
        {
            get { return _peer.Monitor.DownloadSpeed; }
        }

        public long UploadSpeed
        {
            get { return _peer.Monitor.UploadSpeed; }
        }

        public double Progress
        {
            get { return (DownloadedBytes > 0 ? _peer.TorrentManager.Torrent.Size / DownloadedBytes : 0); }
        }
    }
}
