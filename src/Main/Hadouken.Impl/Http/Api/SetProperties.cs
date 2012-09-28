using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.Http.Api
{
    [ApiAction("setprops")]
    public class SetProperties : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public SetProperties(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            var data = BindModel<Dictionary<string, Dictionary<string, object>>>();

            if (data == null)
                return Json(false);

            foreach(var infoHash in data.Keys)
            {
                if(_torrentEngine.Managers.ContainsKey(infoHash))
                {
                    var manager = _torrentEngine.Managers[infoHash];
                    SetData(manager, data[infoHash]);
                }
            }

            return Json(true);
        }

        private void SetData(ITorrentManager manager, Dictionary<string, object> dictionary)
        {
            foreach(var key in dictionary.Keys)
            {
                switch(key)
                {
                    case "label":
                        manager.Label = dictionary[key].ToString();
                        break;
                }
            }
        }
    }
}
