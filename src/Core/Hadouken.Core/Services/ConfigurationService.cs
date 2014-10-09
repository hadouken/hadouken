using System;
using System.Collections.Generic;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;

namespace Hadouken.Core.Services
{
    public sealed class ConfigurationService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigurationService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("config.get")]
        public object Get(string key)
        {
            return _keyValueStore.Get<object>(key);
        }

        [JsonRpcMethod("config.getMany")]
        public IDictionary<string, object> GetMany(string section)
        {
            return _keyValueStore.GetMany(section);
        }

        [JsonRpcMethod("config.set")]
        public void Set(string key, object value)
        {
            _keyValueStore.Set(key, value);
        }

        [JsonRpcMethod("config.setMany")]
        public void SetMany(IDictionary<string, object> items)
        {
            _keyValueStore.SetMany(items);
        }
    }
}
