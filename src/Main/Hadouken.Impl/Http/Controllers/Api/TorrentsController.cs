using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

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

        [HttpPost]
        [Route("/api/torrents")]
        public ActionResult Post()
        {
            foreach (var file in Context.Request.Form.Files)
            {
                _torrentEngine.AddTorrent(file.FileData);
            }

            return Redirect("/");
        }
    }
}
