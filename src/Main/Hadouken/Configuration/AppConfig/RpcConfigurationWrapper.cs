namespace Hadouken.Configuration.AppConfig
{
    public class RpcConfigurationWrapper : IRpcConfiguration
    {
        private readonly RpcConfiguration _configuration;

        public RpcConfigurationWrapper(RpcConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GatewayUri
        {
            get { return _configuration.GatewayUri; }
            set { _configuration.GatewayUri = value; }
        }

        public string PluginUriTemplate
        {
            get { return _configuration.PluginUriTemplate; }
            set { _configuration.PluginUriTemplate = value; }
        }
    }
}
