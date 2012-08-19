using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using System.IO;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("addfile")]
    public class AddFile : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public AddFile(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            List<string> hashes = new List<string>();

            foreach (var file in Context.Request.Files)
            {
                using (var ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    var man = _torrentEngine.AddTorrent(ms.ToArray());

                    if (man != null)
                        hashes.Add(man.InfoHash);
                }
            }

            return Redirect("/api?action=gettorrents&hash=" + String.Join("&hash=", hashes));
        }
    }
}
