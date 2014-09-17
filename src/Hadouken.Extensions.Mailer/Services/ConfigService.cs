using System;
using Hadouken.Common.Data;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Mailer.Config;

namespace Hadouken.Extensions.Mailer.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;

        public ConfigService(IKeyValueStore keyValueStore)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            _keyValueStore = keyValueStore;
        }

        [JsonRpcMethod("mailer.config.get")]
        public MailerConfig GetConfig()
        {
            return _keyValueStore.Get<MailerConfig>("mailer.config");
        }

        [JsonRpcMethod("mailer.config.set")]
        public void SetConfig(MailerConfig config)
        {
            _keyValueStore.Set("mailer.config", config);
        }
    }
}
