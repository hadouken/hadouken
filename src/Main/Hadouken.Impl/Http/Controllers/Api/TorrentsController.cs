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
            return new JsonResult() { Data = _torrentEngine.Torrents };
        }

        [HttpGet]
        [Route("/api/torrents/(?<infoHash>[a-zA-Z0-9]+)")]
        public ActionResult Single(string infoHash)
        {
            return null;
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

                    _torrentEngine.AddTorrent(ms.ToArray());
                }
            }

            return Redirect("/");
        }
    }
}
