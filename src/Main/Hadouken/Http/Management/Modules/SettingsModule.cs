using System;
using Hadouken.Configuration;
using Hadouken.Http.Management.Models;
using Nancy;

namespace Hadouken.Http.Management.Modules
{
    public class SettingsModule : NancyModule
    {
        public SettingsModule(IConfiguration configuration)
            : base("settings")
        {
            Get["/"] = _ =>
            {
                var dto = new GetSettingsItem
                {
                    DataPath = configuration.ApplicationDataPath,
                    HttpHostBinding = configuration.Http.HostBinding,
                    HttpPort = configuration.Http.Port,
                    InstanceName = configuration.InstanceName,
                    PluginsBaseDirectory = configuration.Plugins.BaseDirectory,
                    PluginsRepositoryUri = configuration.Plugins.RepositoryUri,
                    RpcGatewayUri = new Uri(configuration.Rpc.GatewayUri),
                    RpcPluginUriTemplate = configuration.Rpc.PluginUriTemplate
                };

                return View["Index", dto];
            };
        }
    }
}
