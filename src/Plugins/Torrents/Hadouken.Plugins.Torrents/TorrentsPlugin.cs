using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Http;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Torrents.BitTorrent;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsPlugin : Plugin
    {
        private readonly IBitTorrentEngine _torrentEngine;
        private readonly WcfJsonRpcServer _rpcServer;
        private readonly IHttpFileServer _fileServer;

        public TorrentsPlugin(IBitTorrentEngine torrentEngine, WcfJsonRpcServer rpcServer, IHttpFileServer fileServer)
        {
            _torrentEngine = torrentEngine;
            _rpcServer = rpcServer;
            _fileServer = fileServer;
        }

        public override void Load()
        {
            _torrentEngine.Load();
            _rpcServer.Open();
            _fileServer.Open();
        }

        public override void Unload()
        {
            _fileServer.Close();
            _rpcServer.Close();
            _torrentEngine.Unload();
        }
    }
}
