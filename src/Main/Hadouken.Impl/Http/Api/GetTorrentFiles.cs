using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("gettorrentfiles")]
    public class GetTorrentFiles : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public GetTorrentFiles(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            string hash = Context.Request.QueryString["hash"];

            if (!String.IsNullOrEmpty(hash) && _torrentEngine.Managers.ContainsKey(hash))
            {
                return Json(new { files = new object[] {
                    hash,
                    (from f in _torrentEngine.Managers[hash].Torrent.Files
                     select new object[]
                     {
                         f.Path,
                         f.Length,
                         f.BytesDownloaded,
                         f.Priority
                     })
                }
                });
            }

            // no hash not specified or no torrent with specified hash
            // return empty array

            return Json(new object[] { });
        }
    }
}
