using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Pushover.Config;

namespace Hadouken.Extensions.Pushover.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("pushover.config.get")]
        public PushoverConfig GetConfig()
        {
            return _keyValueStore.Get<PushoverConfig>("pushover.config");
        }

        [JsonRpcMethod("pushover.config.set")]
        public void SetConfig(PushoverConfig config)
        {
            _keyValueStore.Set("pushover.config", config);
        }
    }
}
