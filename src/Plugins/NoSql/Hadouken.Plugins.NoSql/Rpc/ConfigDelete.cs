using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.NoSql.Rpc
{
    [RpcMethod("config.delete")]
    public class ConfigDelete : IRpcMethod
    {
        private readonly IConfigStore _configStore;

        public ConfigDelete(IConfigStore configStore)
        {
            _configStore = configStore;
        }

        public void Execute(string key)
        {
            _configStore.Delete(key);
        }
    }
}
