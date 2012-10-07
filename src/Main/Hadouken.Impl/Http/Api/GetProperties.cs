using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("getprops")]
    public class GetProperties : ApiAction
    {
        private readonly IBitTorrentEngine _torrentEngine;

        public GetProperties(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            string hash = Context.Request.QueryString["hash"];

            if(_torrentEngine.Managers.ContainsKey(hash))
            {
                var manager = _torrentEngine.Managers[hash];
                return Json(new[]
                                {
                                    new
                                        {
                                            hash = manager.InfoHash,
                                            trackers = "",
                                            //ulrate = manager.Settings
                                        }
                                });
            }

            return Json(new { props = new [] {new {}} });
        }
    }
}
