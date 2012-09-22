using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("getpeers")]
    public class GetPeers : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public GetPeers(IBitTorrentEngine torrentEngine)
        {
            this._torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            var hash = Context.Request.QueryString["hash"];

            if (hash == null || !_torrentEngine.Managers.ContainsKey(hash))
                return Json(null);

            return Json(new
            {
                peers = (from peer in _torrentEngine.Managers[hash].Peers
                        select new object[]
                        {
                            "00", // country
                            peer.EndPoint.Address.ToString(), // ip
                            peer.ReverseDns, // reverse dns
                            0, // utp
                            peer.EndPoint.Port, // port
                            peer.ClientSoftware, // client software
                            "", // flags
                            peer.Progress, // progress
                            peer.DownloadSpeed, // download speed
                            peer.UploadSpeed, // upload speed
                            -1, // requests in
                            -1, // requests out
                            -1, // waited
                            peer.UploadedBytes, // uploaded
                            peer.DownloadedBytes, // downloaded
                            peer.HashFails, // hash errors
                            -1, // peer dl
                            -1, // max up
                            -1, // max down
                            -1, // queued
                            -1, // inactive
                            -1, // relevance
                        }).ToArray()
            });
        }
    }
}
