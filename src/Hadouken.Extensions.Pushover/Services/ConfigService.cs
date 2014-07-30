using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Pushover.Config;
using Hadouken.Extensions.Pushover.Http;

namespace Hadouken.Extensions.Pushover.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly IPushoverClient _pushoverClient;

        public ConfigService(IKeyValueStore keyValueStore, IPushoverClient pushoverClient)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (pushoverClient == null) throw new ArgumentNullException("pushoverClient");
            _keyValueStore = keyValueStore;
            _pushoverClient = pushoverClient;
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

        [JsonRpcMethod("pushover.config.test")]
        public void TestConfig(PushoverConfig config)
        {
            var message = new PushoverMessage(config.AppKey, config.UserKey, "Test notification from Hadouken");
            _pushoverClient.Send(message);
        }
    }
}
