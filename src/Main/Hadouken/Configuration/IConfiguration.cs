namespace Hadouken.Configuration
{
    public interface IConfiguration
    {
        string WebApplicationPath { get; set; }

        string InstanceName { get; set; }

        string HostBinding { get; set; }

        int Port { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

        string PluginDirectory { get; set; }

        string PluginUrlTemplate { get; set; }

        string PluginRepositoryUrl { get; set; }

        string[] CorePluginPackages { get; }

        bool HasDownloadedCorePluginPackages { get; set; }

        void Save();
    }
}
