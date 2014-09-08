using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.JsonRpc;
using Hadouken.Extensions.Mailer.Config;

namespace Hadouken.Extensions.Mailer.Services
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IKeyValueStore _keyValueStore;
        private readonly IMailSender _mailSender;

        public ConfigService(IKeyValueStore keyValueStore, IMailSender mailSender)
        {
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (mailSender == null) throw new ArgumentNullException("mailSender");
            _keyValueStore = keyValueStore;
            _mailSender = mailSender;
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

        [JsonRpcMethod("mailer.config.test")]
        public void TestConfig(MailerConfig config)
        {
            var notif = new Notification("Hadouken", "Test notification from Hadouken.");
            _mailSender.Send(config, notif);
        }
    }
}
