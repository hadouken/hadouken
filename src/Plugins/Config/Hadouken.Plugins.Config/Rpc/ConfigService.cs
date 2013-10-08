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

        [JsonRpcMethod("config.getMany")]
        public IDictionary<string, object> GetMany(string[] keys)
        {
            if (keys == null || keys.Length == 0)
                return new Dictionary<string, object>();

            return keys.ToDictionary(key => key, Get);
        }

        [JsonRpcMethod("config.set")]
        public bool Set(string key, object val)
        {
            _dataStore.Set(key, val);
            return true;
        }

        [JsonRpcMethod("config.setMany")]
        public bool SetMany(Dictionary<string, object> values)
        {
            if (values == null || !values.Any())
                return false;

            foreach (var val in values)
            {
                Set(val.Key, val.Value);
            }

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
