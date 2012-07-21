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
            var torrents = _torrentEngine.Managers.Values.ToDictionary(s => s.InfoHash, s => new
            {
                Name = s.Torrent.Name,
                InfoHash = s.Torrent.InfoHash,
                Size = s.Torrent.Size,
                DownloadedBytes = s.DownloadedBytes,
                UploadedBytes = s.UploadedBytes,
                DownloadSpeed = s.DownloadSpeed,
                UploadSpeed = s.UploadSpeed,
                Progress = s.Progress,
                Label = s.Label,
                State = s.State
                //PeersCount = manager.,
                //Seeders = torrent.Peers.Where(p => p.IsSeeder).Count()
            });

            // labels

            return Json(new { torrents = torrents });
        }

        [HttpGet]
        [Route("/api/torrents/(?<infoHash>[a-zA-Z0-9]+)")]
        public ActionResult Single(string infoHash)
        {
            if (_torrentEngine.Managers.ContainsKey(infoHash))
                return Json(_torrentEngine.Managers[infoHash]);

            return Json(null);
        }

        [HttpPut]
        [Route("/api/torrents/(?<infoHash>[a-zA-Z0-9]+)")]
        public ActionResult Change(string infoHash)
        {
            ITorrentManager manager = null;

            if (_torrentEngine.Managers.TryGetValue(infoHash, out manager))
            {
                Dictionary<string, string> actions = BindModel<Dictionary<string, string>>();

                if (actions.ContainsKey("Action"))
                {
                    switch (actions["Action"])
                    {
                        case "Start":
                            manager.Start();
                            break;

                        case "Stop":
                            manager.Stop();
                            break;

                        case "Pause":
                            manager.Pause();
                            break;
                    }
                }

                return Json(true);
            }

            return Json(false);
        }

        [HttpDelete]
        [Route("/api/torrents/(?<infoHash>[a-zA-Z0-9]+)")]
        public ActionResult Remove(string infoHash)
        {
            if (_torrentEngine.Managers.ContainsKey(infoHash))
            {
                _torrentEngine.RemoveTorrent(_torrentEngine.Managers[infoHash]);
                return Json(true);
            }

            return Json(false);
        }

        [HttpPost]
        [Route("/api/torrents")]
        public ActionResult Post()
        {
            List<string> hashes = new List<string>();

            foreach (var file in Context.Request.Files)
            {
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    var manager = _torrentEngine.AddTorrent(ms.ToArray());

                    hashes.Add(manager.InfoHash);
                }
            }

            return Json(hashes);
        }
    }
}
