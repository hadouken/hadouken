using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Configuration;

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
            throw new NotImplementedException();
        }
    }
}
