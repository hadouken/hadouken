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

            if (_torrentEngine.Managers.ContainsKey(hash))
            {
                var manager = _torrentEngine.Managers[hash];
                return Json(new
                                {
                                    props = new[]
                                                {
                                                    new
                                                        {
                                                            hash = manager.InfoHash,
                                                            trackers = "",
                                                            ulrate = manager.Settings.MaxUploadSpeed,
                                                            dlrate = manager.Settings.MaxDownloadSpeed,
                                                            superseed = manager.IsInitialSeeding,
                                                            dht = manager.Settings.UseDht,
                                                            pex = manager.Settings.EnablePeerExchange,
                                                            seed_override = false,
                                                            seed_ratio = 0,
                                                            seed_time = 0,
                                                            ulslots = manager.Settings.UploadSlots,
                                                            seed_num = 0
                                                        }
                                                }
                                });
            }

            return Json(new { props = new [] {new {}} });
        }
    }
}
