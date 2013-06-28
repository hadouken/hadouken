using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Configuration;
using Hadouken.Security;

namespace Hadouken.Http.Api
{
    [ApiAction("setcredentials")]
    public class SetCredentials : ApiAction
    {
        private readonly IKeyValueStore _keyValueStore;

        public SetCredentials(IKeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
        }

        public override ActionResult Execute()
        {
            var model = BindModel<Dictionary<string, string>>();

            if (!model.ContainsKey("oldPassword"))
                return Json(false);

            var oldPassword = Hash.Generate(model["oldPassword"]);
            var currentPassword = _keyValueStore.Get<string>("auth.password");

            if (!String.Equals(oldPassword, currentPassword, StringComparison.InvariantCultureIgnoreCase))
                return Json(false);

            if (model.ContainsKey("username"))
            {
                _keyValueStore.Set("auth.username", model["username"]);
            }

            if (model.ContainsKey("password"))
            {
                var hashedPassword = Hash.Generate(model["password"]);
                _keyValueStore.Set("auth.password", hashedPassword);
            }

            return Json(true);
        }
    }
}
