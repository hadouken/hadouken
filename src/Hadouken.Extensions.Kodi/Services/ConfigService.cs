using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Kodi.Config;

namespace Hadouken.Extensions.Kodi.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("kodi.config.get")]
        public KodiConfig GetConfig()
        {
            return _keyValueStore.Get<KodiConfig>("kodi.config");
        }

        [JsonRpcMethod("kodi.config.set")]
        public void SetConfig(KodiConfig config)
        {
            _keyValueStore.Set("kodi.config", config);
        }
    }
}
