using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.HipChat.Config;

namespace Hadouken.Extensions.HipChat.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("hipchat.config.get")]
        public HipChatConfig GetConfig()
        {
            return _keyValueStore.Get<HipChatConfig>("hipchat.config");
        }

        [JsonRpcMethod("hipchat.config.set")]
        public void SetConfig(HipChatConfig config)
        {
            _keyValueStore.Set("hipchat.config", config);
        }
    }
}
