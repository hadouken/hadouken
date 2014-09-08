using System.Net;
using System.Net.Mail;

namespace Hadouken.Common.Net
{
    public interface ISmtpClient
    {
        bool EnableSsl { get; set; }

        ICredentialsByHost Credentials { get; set; }

        void Send(MailMessage message);
    }
}
