using System;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Extensions.Mailer.Config;

namespace Hadouken.Extensions.Mailer {
    [Extension("notifier.mailer",
        Name = "Mailer",
        Description = "Sends e-mails to a specified address."
        )]
    [Configuration(typeof (MailerConfig), Key = "mailer.config")]
    public class MailerNotifier : INotifier {
        private readonly IKeyValueStore _keyValueStore;
        private readonly ILogger<MailerNotifier> _logger;
        private readonly IMailSender _mailSender;

        public MailerNotifier(ILogger<MailerNotifier> logger,
            IKeyValueStore keyValueStore,
            IMailSender mailSender) {
            if (logger == null) {
                throw new ArgumentNullException("logger");
            }
            if (keyValueStore == null) {
                throw new ArgumentNullException("keyValueStore");
            }
            if (mailSender == null) {
                throw new ArgumentNullException("mailSender");
            }

            this._logger = logger;
            this._keyValueStore = keyValueStore;
            this._mailSender = mailSender;
        }

        public bool CanNotify() {
            var config = this._keyValueStore.Get<MailerConfig>("mailer.config");

            return (config != null
                    && !string.IsNullOrEmpty(config.From)
                    && !string.IsNullOrEmpty(config.Host)
                    && !string.IsNullOrEmpty(config.Password)
                    && !string.IsNullOrEmpty(config.UserName));
        }

        public void Notify(Notification notification) {
            var config = this._keyValueStore.Get<MailerConfig>("mailer.config");

            if (config == null) {
                this._logger.Warn("Mailer not configured.");
                return;
            }

            this._mailSender.Send(config, notification);
        }
    }
}