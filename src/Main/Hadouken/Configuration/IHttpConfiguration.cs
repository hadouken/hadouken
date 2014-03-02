namespace Hadouken.Configuration
{
    public interface IHttpConfiguration
    {
        string HostBinding { get; set; }

        int Port { get; set; }

        IHttpAuthConfiguration Authentication { get; }
    }
}
