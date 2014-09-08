namespace Hadouken.Common.Net
{
    public class SmtpClientFactory : ISmtpClientFactory
    {
        public ISmtpClient Create(string host, int port)
        {
            return new SmtpClientWrapper(host, port);
        }
    }
}