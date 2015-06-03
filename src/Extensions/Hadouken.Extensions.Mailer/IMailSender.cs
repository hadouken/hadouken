using Hadouken.Common.Extensibility.Notifications;
using Hadouken.Extensions.Mailer.Config;

namespace Hadouken.Extensions.Mailer {
    public interface IMailSender {
        void Send(MailerConfig config, Notification notification);
    }
}