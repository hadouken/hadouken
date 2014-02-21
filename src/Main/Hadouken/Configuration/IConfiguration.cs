namespace Hadouken.Configuration
{
    public interface IConfiguration
    {
        string ApplicationDataPath { get; set; }

        string InstanceName { get; set; }

        IPluginConfigurationCollection Plugins { get; }

        IHttpConfiguration Http { get; }

        IRpcConfiguration Rpc { get; }

        void Save();
    }
}
