using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("remove")]
    public class RemoveTorrent : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public RemoveTorrent(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            string[] hashes = BindModel<string[]>();

            foreach (var hash in hashes)
            {
                if (_torrentEngine.Managers.ContainsKey(hash))
                    _torrentEngine.RemoveTorrent(_torrentEngine.Managers[hash]);
            }

            return Json(true);
        }
    }
}
