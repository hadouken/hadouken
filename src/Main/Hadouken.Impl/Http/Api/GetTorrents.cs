using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;
using System.IO;

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

        public override ActionResult Execute()
        {
            if (_torrentEngine.Managers == null)
                return Json(new {labels = new object[] {}, torrents = new object[] {}});

            return Json(new
            {
                label = (from l in _torrentEngine.Managers.Values.GroupBy(m => m.Label)
                         where !String.IsNullOrEmpty(l.Key)
                             select new object[]
                                         {
                                             l.Key,
                                             l.Count()
                                         }),
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
                               (t.DownloadedBytes == 0 ? 0 : (int)((t.UploadedBytes / t.DownloadedBytes) * 10)),
                               t.UploadSpeed,
                               t.DownloadSpeed,
                               t.ETA.TotalSeconds,
                               t.Label,
                               t.Peers.Count(p => !p.IsSeeder),
                               t.Trackers.Sum(tr => tr.Incomplete),
                               t.Peers.Count(p => p.IsSeeder),
                               t.Trackers.Sum(tr => tr.Complete),
                               -1, // availability
                               -1, // queue position
                               t.RemainingBytes,
                               "", // download url
                               "", // rss feed url
                               (t.State == TorrentState.Error ? "Error: --" : ""),
                               -1, // stream id
                               t.StartTime.ToUnixTime(),
                               (t.CompletedTime.HasValue ? t.CompletedTime.Value.ToUnixTime() : -1),
                               "", // app update url
                               Path.Combine(t.SavePath, t.Torrent.Name),
                               t.Complete
                           })
            });
        }
    }
}
