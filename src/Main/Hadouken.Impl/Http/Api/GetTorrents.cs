using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("gettorrents")]
    public class GetTorrents : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public GetTorrents(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute(IHttpContext context)
        {
            return Json(new
            {
                labels = new object[] {
                },
                torrents= (from t in _torrentEngine.Managers.Values
                           select new object[]
                           {
                               t.InfoHash,
                               t.State,
                               t.Torrent.Name,
                               t.Torrent.Size,
                               (int)t.Progress * 10,
                               t.DownloadedBytes,
                               t.UploadedBytes,
                               (t.DownloadedBytes == 0 ? 0 : (int)((t.UploadedBytes / t.DownloadedBytes) * 10))
                           })
            });
        }
    }
}
