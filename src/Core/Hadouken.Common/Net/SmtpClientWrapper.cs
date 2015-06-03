using System;
using System.Net;
using System.Net.Mail;

namespace Hadouken.Common.Net {
    public class SmtpClientWrapper : ISmtpClient {
        private readonly string _host;
        private readonly int _port;

        public SmtpClientWrapper(string host, int port) {
            if (host == null) {
                throw new ArgumentNullException("host");
            }

            this._host = host;
            this._port = port;
        }

        public bool EnableSsl { get; set; }
        public ICredentialsByHost Credentials { get; set; }

        public void Send(MailMessage message) {
            using (var client = new SmtpClient(this._host, this._port)) {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = this.EnableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = this.Credentials;

                client.Send(message);
            }
        }
    }
}