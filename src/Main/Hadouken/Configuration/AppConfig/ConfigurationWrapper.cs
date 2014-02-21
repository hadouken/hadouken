using System;

namespace Hadouken.Configuration.AppConfig
{
    public class ConfigurationWrapper : IConfiguration
    {
        private readonly HadoukenConfigurationSection _configSection;
        private readonly PluginConfigurationCollectionWrapper _pluginConfiguration;
        private readonly HttpConfigurationWrapper _httpConfiguration;
        private readonly RpcConfigurationWrapper _rpcConfiguration;

        public ConfigurationWrapper(HadoukenConfigurationSection configSection)
        {
            _configSection = configSection;
            _pluginConfiguration = new PluginConfigurationCollectionWrapper(configSection.Plugins);
            _httpConfiguration = new HttpConfigurationWrapper(configSection.Http);
            _rpcConfiguration = new RpcConfigurationWrapper(configSection.Rpc);
        }

        public string ApplicationDataPath
        {
            get { return _configSection.ApplicationDataPath; }
            set { _configSection.ApplicationDataPath = value; }
        }

        public string InstanceName
        {
            get { return _configSection.InstanceName; }
            set { _configSection.InstanceName = value; }
        }

        public IPluginConfigurationCollection Plugins
        {
            get { return _pluginConfiguration; }
        }

        public IHttpConfiguration Http
        {
            get { return _httpConfiguration; }
        }

        public IRpcConfiguration Rpc
        {
            get { return _rpcConfiguration; }
        }

        public void Save()
        {
            _configSection.Save();
        }
    }
}
