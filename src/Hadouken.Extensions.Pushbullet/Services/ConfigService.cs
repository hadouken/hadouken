using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Pushbullet.Config;

namespace Hadouken.Extensions.Pushbullet.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("pushbullet.config.get")]
        public PushbulletConfig GetConfig()
        {
            return _keyValueStore.Get<PushbulletConfig>("pushbullet.config");
        }

        [JsonRpcMethod("pushbullet.config.set")]
        public void SetConfig(PushbulletConfig config)
        {
            _keyValueStore.Set("pushbullet.config", config);
        }
    }
}
