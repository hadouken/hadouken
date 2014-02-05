using System;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Wcf;

namespace Hadouken.Framework.Rpc
{
    public class WcfClientTransport : IClientTransport
    {
        private readonly IProxyFactory<IPluginManagerService> _proxyFactory;
        private readonly Uri _endpoint;

        public WcfClientTransport(IProxyFactory<IPluginManagerService> proxyFactory, IBootConfig bootConfig)
            : this(proxyFactory, new Uri(bootConfig.RpcGatewayUri))
        {
        }

        public WcfClientTransport(IProxyFactory<IPluginManagerService> proxyFactory, Uri endpoint)
        {
            _proxyFactory = proxyFactory;
            _endpoint = endpoint;
        }

        public string Send(string data)
        {
            using (var proxy = _proxyFactory.Create(_endpoint))
            {
                return proxy.Channel.RpcAsync(data).Result;
            }
        }
    }
}
