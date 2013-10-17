using System;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Wcf;

namespace Hadouken.Framework.Rpc
{
    public class WcfClientTransport : IClientTransport
    {
        private readonly IProxy<IPluginManagerService> _pluginManagerService;

        public WcfClientTransport(IProxyFactory<IPluginManagerService> proxyFactory, IBootConfig bootConfig)
            : this(proxyFactory, new Uri(bootConfig.RpcGatewayUri))
        {
        }

        public WcfClientTransport(IProxyFactory<IPluginManagerService> proxyFactory, Uri endpoint)
        {
            _pluginManagerService = proxyFactory.Create(endpoint);
        }

        public string Send(string data)
        {
            return _pluginManagerService.Channel.RpcAsync(data).Result;
        }
    }
}
