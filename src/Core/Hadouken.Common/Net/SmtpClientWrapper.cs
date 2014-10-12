using System;
using System.Net;
using System.Net.Mail;

namespace Hadouken.Common.Net
{
    public class SmtpClientWrapper : ISmtpClient
    {
        private readonly string _host;
        private readonly int _port;

        public SmtpClientWrapper(string host, int port)
        {
            if (host == null) throw new ArgumentNullException("host");

            _host = host;
            _port = port;
        }

        public bool EnableSsl { get; set; }

        public ICredentialsByHost Credentials { get; set; }

        public void Send(MailMessage message)
        {
            using (var client = new SmtpClient(_host, _port))
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = EnableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = Credentials;

                client.Send(message);
            }
        }
    }
}