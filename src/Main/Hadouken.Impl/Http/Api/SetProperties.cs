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
                object value = dictionary[key];

                switch(key)
                {
                    case "label":
                        manager.Label = value.ToString();
                        break;

                    case "ulrate":
                        manager.Settings.MaxUploadSpeed = Convert.ToInt32(value);
                        break;

                    case "dlrate":
                        manager.Settings.MaxDownloadSpeed = Convert.ToInt32(value);
                        break;

                    case "superseed":
                        manager.Settings.InitialSeedingEnabled = Convert.ToBoolean(value);
                        break;

                    case "dht":
                        manager.Settings.UseDht = Convert.ToBoolean(value);
                        break;

                    case "pex":
                        manager.Settings.EnablePeerExchange = Convert.ToBoolean(value);
                        break;

                    case "ulslots":
                        manager.Settings.UploadSlots = Convert.ToInt32(value);
                        break;
                }
            }
        }
    }
}
