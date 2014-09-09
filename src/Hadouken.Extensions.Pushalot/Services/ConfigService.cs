using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Pushalot.Config;
using Hadouken.Extensions.Pushalot.Http;

namespace Hadouken.Extensions.Pushalot.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly IPushalotClient _pushalotClient;

        public ConfigService(IKeyValueStore keyValueStore, IPushalotClient pushalotClient)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (pushalotClient == null) throw new ArgumentNullException("pushalotClient");
            _keyValueStore = keyValueStore;
            _pushalotClient = pushalotClient;
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

        [JsonRpcMethod("pushalot.config.test")]
        public void TestConfig(PushalotConfig config)
        {
            var msg = new Message(config.AuthorizationToken, "Test notification from Hadouken.")
            {
                Title = "Hadouken"
            };

            _pushalotClient.Send(msg);
        }
    }
}
