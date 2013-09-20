using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.NoSql.Rpc
{
    public class ConfigService : IJsonRpcService
    {
        private readonly IConfigStore _configStore;

        public ConfigService(IConfigStore configStore)
        {
            _configStore = configStore;
        }

        [JsonRpcMethod("config.get")]
        public object Get(string key)
        {
            return _configStore.Get(key);
        }

        [JsonRpcMethod("config.set")]
        public bool Set(string key, object value)
        {
            _configStore.Set(key, value);
            return true;
        }

        [JsonRpcMethod("config.delete")]
        public bool Delete(string key)
        {
            _configStore.Delete(key);
            return true;
        }
    }
}
