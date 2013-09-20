using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Config.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Config.Rpc
{
    public class ConfigService : IJsonRpcService
    {
        private readonly IConfigDataStore _dataStore;

        public ConfigService(IConfigDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [JsonRpcMethod("config.get")]
        public object Get(string key)
        {
            return _dataStore.Get(key);
        }

        [JsonRpcMethod("config.set")]
        public bool Set(string key, object val)
        {
            _dataStore.Set(key, val);
            return true;
        }

        [JsonRpcMethod("config.delete")]
        public bool Delete(string key)
        {
            _dataStore.Delete(key);
            return true;
        }
    }
}
