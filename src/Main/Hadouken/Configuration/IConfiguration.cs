namespace Hadouken.Configuration
{
    public interface IConfiguration
    {
        string ApplicationDataPath { get; set; }

        string WebApplicationPath { get; set; }

        string InstanceName { get; set; }

        string HostBinding { get; set; }

        int Port { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

        string PluginDirectory { get; set; }

        string PluginUrlTemplate { get; set; }

        string PluginRepositoryUrl { get; set; }

        void Save();
    }
}
