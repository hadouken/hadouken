using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Http;
using Hadouken.BitTorrent;
using System.Net;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("addurl")]
    public class AddUrl : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public AddUrl(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            var data = BindModel<Dictionary<string, string>>();

            if (data == null || !data.ContainsKey("url"))
                return Json(false);

            var url = data["url"];
            var isMagnet = url.StartsWith("magnet:?");
            
            if(isMagnet)
            {
                _torrentEngine.AddMagnetLink(url);
            }
            else
            {
                new Task(() =>
                             {
                                 var torrentData = new WebClient().DownloadData(url);
                                 _torrentEngine.AddTorrent(torrentData);
                             }).Start();
            }

            return Json(true);
        }
    }
}
