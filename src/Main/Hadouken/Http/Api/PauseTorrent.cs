using Hadouken.BitTorrent;

namespace Hadouken.Http.Api
{
    [ApiAction("pause")]
    public class PauseTorrent : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public PauseTorrent(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            string[] hashes = BindModel<string[]>();

            foreach (var hash in hashes)
            {
                if (_torrentEngine.Managers.ContainsKey(hash))
                    _torrentEngine.Managers[hash].Pause();
            }

            return Json(true);
        }
    }
}
