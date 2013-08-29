using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.NoSql.Rpc
{
    [RpcMethod("config.get")]
    public class ConfigGet : IRpcMethod
    {
        private readonly IConfigStore _configStore;

        public ConfigGet(IConfigStore configStore)
        {
            _configStore = configStore;
        }

        public object Execute(string key)
        {
            return _configStore.Get(key);
        }
    }
}
