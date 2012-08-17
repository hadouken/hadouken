using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Http;
using Hadouken.Data;
using Hadouken.Data.Models;
using Hadouken.Messaging;
using Hadouken.Messages;
using Hadouken.Configuration;

namespace Hadouken.Impl.Http.Controllers.Api
{
    public class SettingsController : Controller
    {
        private IKeyValueStore _kvs;
        private IMessageBus _mbus;

        public SettingsController(IKeyValueStore kvs, IMessageBus mbus)
        {
            _kvs = kvs;
            _mbus = mbus;
        }

        [HttpGet]
        [Route("/api/settings")]
        public ActionResult Get()
        {
            IDictionary<string, object> dict = null;

            if (!String.IsNullOrEmpty(Context.Request.QueryString["k"]))
            {
                string[] keys = Context.Request.QueryString["k"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                dict = _kvs.Get(s => keys.Contains(s));
            }

            if (!String.IsNullOrEmpty(Context.Request.QueryString["g"]))
            {
                string group = Context.Request.QueryString["g"];
                dict = _kvs.Get(s => s.StartsWith(group));
            }

            if (dict == null)
                dict = _kvs.Get(s => true);

            return Json(new {
                settings = (from d in dict
                            select new {
                                name = d.Key,
                                type = "",
                                value = d.Value,
                                param = ""
                            })
            });
        }

        [HttpPost]
        [Route("/api/settings")]
        public ActionResult Change()
        {
            var dictionary = BindModel<Dictionary<string, object>>();

            foreach (var key in dictionary.Keys)
            {
                _kvs.Set(key, dictionary[key]);

                //_mbus.Send<ISettingChanged>(m => { m.Key = key; m.NewValue = dictionary[key]; });
            }

            return Json(true);
        }
    }
}
