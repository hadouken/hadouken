using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Pushalot.Config;

namespace Hadouken.Extensions.Pushalot.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("pushalot.config.get")]
        public PushalotConfig GetConfig()
        {
            return _keyValueStore.Get<PushalotConfig>("pushalot.config");
        }

        [JsonRpcMethod("pushalot.config.set")]
        public void SetConfig(PushalotConfig config)
        {
            _keyValueStore.Set("pushalot.config", config);
        }
    }
}
