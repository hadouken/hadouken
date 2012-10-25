using System.Collections.Generic;
using Hadouken.Configuration;

namespace Hadouken.Http.Api
{
    [ApiAction("setsetting")]
    public class SetSetting : ApiAction
    {
        private IKeyValueStore _keyValueStore;

        public SetSetting(IKeyValueStore keyValueStore)
        {
            this._keyValueStore = keyValueStore;
        }

        public override ActionResult Execute()
        {
            var data = BindModel<Dictionary<string, object>>();

            if (data == null)
                return Json(false);

            foreach (var key in data.Keys)
            {
                _keyValueStore.Set(key, data[key]);
            }

            return Json(true);
        }
    }
}
