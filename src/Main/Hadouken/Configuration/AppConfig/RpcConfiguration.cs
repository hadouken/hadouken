using System.Configuration;

namespace Hadouken.Configuration.AppConfig
{
    public sealed class RpcConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("gatewayUri", IsRequired = true)]
        public string GatewayUri
        {
            get { return this["gatewayUri"].ToString(); }
            set { this["gatewayUri"] = value; }
        }

        [ConfigurationProperty("pluginUriTemplate", IsRequired = true)]
        public string PluginUriTemplate
        {
            get { return this["pluginUriTemplate"].ToString(); }
            set { this["pluginUriTemplate"] = value; }
        }
    }
}