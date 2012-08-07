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
    public class ConfigController : Controller
    {
        private IKeyValueStore _kvs;
        private IMessageBus _mbus;

        public ConfigController(IKeyValueStore kvs, IMessageBus mbus)
        {
            _kvs = kvs;
            _mbus = mbus;
        }

        [HttpGet]
        [Route("/api/config")]
        public ActionResult GetConfig()
        {
            if (!String.IsNullOrEmpty(Context.Request.QueryString["k"]))
            {
                string[] keys = Context.Request.QueryString["k"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                return Json(_kvs.Get(s => keys.Contains(s)));
            }

            if (!String.IsNullOrEmpty(Context.Request.QueryString["g"]))
            {
                string group = Context.Request.QueryString["g"];
                return Json(_kvs.Get(s => s.StartsWith(group)));
            }

            return Json(_kvs.Get(s => true));
        }

        [HttpPost]
        [Route("/api/config")]
        public ActionResult ChangeConfig()
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
