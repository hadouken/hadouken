using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Config.Data;
using System.Collections.Generic;
using System.Linq;

namespace Hadouken.Plugins.Config.Rpc
{
    public class ConfigService : IJsonRpcService
    {
        private readonly IConfigDataStore _dataStore;
        private readonly IJsonRpcClient _rpcClient;

        public ConfigService(IConfigDataStore dataStore, IJsonRpcClient rpcClient)
        {
            _dataStore = dataStore;
            _rpcClient = rpcClient;
        }

        [JsonRpcMethod("config.get")]
        public object Get(string key)
        {
            return _dataStore.Get(key);
        }

        [JsonRpcMethod("config.getSection")]
        public IDictionary<string, object> GetSection(string section)
        {
            return _dataStore.GetStartingWith(section);
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

            PublishChangedEvent(key);

            return true;
        }

        [JsonRpcMethod("config.setMany")]
        public bool SetMany(Dictionary<string, object> values)
        {
            if (values == null || !values.Any())
                return false;

            foreach (var val in values)
            {
                _dataStore.Set(val.Key, val.Value);
            }

            PublishChangedEvent(values.Keys.ToArray());

            return true;
        }

        [JsonRpcMethod("config.delete")]
        public bool Delete(string key)
        {
            _dataStore.Delete(key);
            return true;
        }

        private void PublishChangedEvent(params string[] keys)
        {
            var data = keys ?? new string[] {};
            _rpcClient.SendEventAsync("config.changed", data);
        }
    }
}
