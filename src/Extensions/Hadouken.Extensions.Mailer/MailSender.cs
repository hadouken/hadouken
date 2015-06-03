using System;
using System.Net;
using System.Net.Mail;
using Hadouken.Common.Extensibility;
using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Common.Net;
using Hadouken.Extensions.Mailer.Config;

namespace Hadouken.Extensions.Mailer {
    [Component(ComponentLifestyle.Singleton)]
    public class MailSender : IMailSender {
        private readonly ISmtpClientFactory _smtpFactory;

        public MailSender(ISmtpClientFactory smtpFactory) {
            if (smtpFactory == null) {
                throw new ArgumentNullException("smtpFactory");
            }
            this._smtpFactory = smtpFactory;
        }

        public void Send(MailerConfig config, Notification notification) {
            var client = this._smtpFactory.Create(config.Host, config.Port);
            client.Credentials = new NetworkCredential(config.UserName, config.Password);
            client.EnableSsl = config.EnableSsl;

            using (var message = new MailMessage(config.From, config.To)) {
                message.Subject = notification.Title;
                message.Body = notification.Message;

                client.Send(message);
            }
        }
    }
}