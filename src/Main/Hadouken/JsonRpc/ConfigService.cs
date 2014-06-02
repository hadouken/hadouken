using Hadouken.Configuration;
using Hadouken.Fx.JsonRpc;
using Hadouken.JsonRpc.Dto;

namespace Hadouken.JsonRpc
{
    public sealed class ConfigService : IJsonRpcService
    {
        private readonly IConfiguration _configuration;

        public ConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [JsonRpcMethod("core.config.get")]
        public object GetConfig()
        {
            return _configuration;
        }

        [JsonRpcMethod("core.config.set")]
        public void SetConfig(ConfigurationItem config)
        {
            _configuration.HostBinding = config.HostBinding;
            _configuration.InstanceName = config.InstanceName;
            _configuration.PluginDirectory = config.PluginDirectory;
            _configuration.PluginRepositoryUrl = config.PluginRepositoryUrl;
            _configuration.PluginUrlTemplate = config.PluginUrlTemplate;
            _configuration.Port = config.Port;

            _configuration.Save();
        }
    }
}
