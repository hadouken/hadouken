using System;

namespace Hadouken.Http.Management.Models
{
    public class GetSettingsItem
    {
        public string InstanceName { get; set; }

        public string DataPath { get; set; }

        public string HttpHostBinding { get; set; }

        public int HttpPort { get; set; }

        public Uri RpcGatewayUri { get; set; }

        public string RpcPluginUriTemplate { get; set; }

        public string PluginsBaseDirectory { get; set; }

        public Uri PluginsRepositoryUri { get; set; }
    }
}
