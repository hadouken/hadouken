using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;
using System.IO;

namespace Hadouken.Impl.Http.Controllers.Api
{
    public class TorrentsController : Controller
    {
        private IBitTorrentEngine _torrentEngine;

        public TorrentsController(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        [HttpGet]
        [Route("/api/torrents")]
        public ActionResult List()
        {
            var torrents = (from manager in _torrentEngine.Managers.Values
                            let torrent = manager.Torrent
                            select new
                            {
                                Name = torrent.Name,
                                InfoHash = torrent.InfoHash,
                                Size = torrent.Size,
                                DownloadedBytes = manager.DownloadedBytes,
                                UploadedBytes = manager.UploadedBytes,
                                DownloadSpeed = manager.DownloadSpeed,
                                UploadSpeed = manager.UploadSpeed,
                                Progress = manager.Progress,
                                Label = manager.Label,
                                State = manager.State
                                //PeersCount = manager.,
                                //Seeders = torrent.Peers.Where(p => p.IsSeeder).Count()
                            });

            return Json(torrents);
        }

        [HttpGet]
        [Route("/api/torrents/(?<infoHash>[a-zA-Z0-9]+)")]
        public ActionResult Single(string infoHash)
        {
            if (_torrentEngine.Managers.ContainsKey(infoHash))
                return Json(_torrentEngine.Managers[infoHash]);

            return Json(null);
        }

        [HttpPost]
        [Route("/api/torrents")]
        public ActionResult Post()
        {
            foreach (var file in Context.Request.Files)
            {
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    _torrentEngine.CreateManager(ms.ToArray());
                }
            }

            return Redirect("/");
        }
    }
}
