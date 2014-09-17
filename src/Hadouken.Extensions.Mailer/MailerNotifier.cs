using System;
using System.Net;
using System.Net.Mail;
using Hadouken.Common.Data;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Logging;
using Hadouken.Common.Net;
using Hadouken.Extensions.Mailer.Config;

namespace Hadouken.Extensions.Mailer
{
    [Extension("notifier.mailer",
        Name = "Mailer",
        Description = "Sends e-mails to a specified address.",
        ResourceNamespace = "Hadouken.Extensions.Mailer.Resources",
        Scripts = new[] { "js/app.js", "js/controllers/settingsController.js" }
    )]
    public class MailerNotifier : INotifier
    {
        private readonly ILogger<MailerNotifier> _logger;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IMailSender _mailSender;

        public MailerNotifier(ILogger<MailerNotifier> logger,
            IKeyValueStore keyValueStore,
            IMailSender mailSender)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (keyValueStore == null) throw new ArgumentNullException("keyValueStore");
            if (mailSender == null) throw new ArgumentNullException("mailSender");

            _logger = logger;
            _keyValueStore = keyValueStore;
            _mailSender = mailSender;
        }

        public bool CanNotify()
        {
            var config = _keyValueStore.Get<MailerConfig>("mailer.config");

            return (config != null
                    && !string.IsNullOrEmpty(config.From)
                    && !string.IsNullOrEmpty(config.Host)
                    && !string.IsNullOrEmpty(config.Password)
                    && !string.IsNullOrEmpty(config.UserName));
        }

        public void Notify(Notification notification)
        {
            var config = _keyValueStore.Get<MailerConfig>("mailer.config");

            if (config == null)
            {
                _logger.Warn("Mailer not configured.");
                return;
            }

            _mailSender.Send(config, notification);
        }
    }
}
