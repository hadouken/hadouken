using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Pushbullet.Config;
using Hadouken.Extensions.Pushbullet.Http;

namespace Hadouken.Extensions.Pushbullet.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly IPushbulletClient _pushbulletClient;

        public ConfigService(IKeyValueStore keyValueStore, IPushbulletClient pushbulletClient)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (pushbulletClient == null) throw new ArgumentNullException("pushbulletClient");
            _keyValueStore = keyValueStore;
            _pushbulletClient = pushbulletClient;
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

        [JsonRpcMethod("pushbullet.config.test")]
        public void TestConfig(PushbulletConfig config)
        {
            var note = new Note("Hadouken", "Test notification from Hadouken.");
            _pushbulletClient.Send(config.AccessToken, note);
        }
    }
}
