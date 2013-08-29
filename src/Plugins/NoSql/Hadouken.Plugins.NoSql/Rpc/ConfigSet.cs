using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.NoSql.Rpc
{
    [RpcMethod("config.set")]
    public class ConfigSet : IRpcMethod
    {
        private readonly IConfigStore _configStore;

        public ConfigSet(IConfigStore configStore)
        {
            _configStore = configStore;
        }

        public void Execute(string key, object value)
        {
            _configStore.Set(key, value);
        }
    }
}
