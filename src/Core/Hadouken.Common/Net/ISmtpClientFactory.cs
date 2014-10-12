namespace Hadouken.Common.Net
{
    public interface ISmtpClientFactory
    {
        ISmtpClient Create(string host, int port);
    }
}
