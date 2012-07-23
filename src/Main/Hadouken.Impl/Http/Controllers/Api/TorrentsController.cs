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
            var torrents = _torrentEngine.Managers.Values.ToDictionary(s => s.InfoHash, s =>
            {
                var peersAll = s.Trackers.Sum(t => t.Incomplete);
                var peersActual = s.Peers.Count(p => !p.IsSeeder);

                var seedsAll = s.Trackers.Sum(t => t.Complete);
                var seedsActual = s.Peers.Count(p => p.IsSeeder);

                var eta = s.ETA.TotalSeconds;

                return new
                    {
                        Name = s.Torrent.Name,
                        InfoHash = s.Torrent.InfoHash,
                        Size = s.Torrent.Size,
                        DownloadedBytes = s.DownloadedBytes,
                        UploadedBytes = s.UploadedBytes,
                        DownloadSpeed = s.DownloadSpeed,
                        UploadSpeed = s.UploadSpeed,
                        Progress = s.Progress,
                        Complete = s.Complete,
                        Label = s.Label,
                        State = s.State,
                        Peers_Info = peersActual + " (" + peersAll + ")",
                        Peers_All = peersAll,
                        Peers_Actual = peersActual,
                        Seeders_Info = seedsActual + " (" + seedsAll + ")",
                        Seeders_All = seedsAll,
                        Seeders_Actual = seedsActual,
                        ETA = eta,
                        CreatedOn = s.Torrent.CreationDate.ToUnixTime()
                    };
            });

            // labels
            var disctinctLabels = (from man in _torrentEngine.Managers.Values where !String.IsNullOrEmpty(man.Label) select man.Label).Distinct();

            var labels = (from lbl in disctinctLabels
                          let cnt = _torrentEngine.Managers.Values.Where(m => m.Label == lbl).Count()
                          select new
                          {
                              Label = lbl,
                              Count = cnt
                          }).ToDictionary(s => s.Label, s => s.Count);

            return Json(new { labels = labels, torrents = torrents });
        }

        [HttpPut]
        [Route("/api/torrents")]
        public ActionResult Edit()
        {
            var actions = BindModel<Dictionary<string, Dictionary<string, string>>>();

            foreach (var p in actions)
            {
                if (_torrentEngine.Managers.ContainsKey(p.Key))
                {
                    var man = _torrentEngine.Managers[p.Key];

                    foreach (var a in p.Value)
                    {
                        switch (a.Key)
                        {
                            case "action":
                                PerformAction(man, a.Value);
                                break;

                            case "label":
                                man.Label = a.Value;
                                break;
                        }
                    }
                }
            }

            return Json(true);
        }

        private void PerformAction(ITorrentManager manager, string action)
        {
            switch (action.ToLowerInvariant())
            {
                case "start":
                    manager.Start();
                    break;

                case "stop":
                    manager.Stop();
                    break;

                case "pause":
                    manager.Pause();
                    break;
            }
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

                if (actions.ContainsKey("action"))
                {
                    switch (actions["action"])
                    {
                        case "start":
                            manager.Start();
                            break;

                        case "stop":
                            manager.Stop();
                            break;

                        case "pause":
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
