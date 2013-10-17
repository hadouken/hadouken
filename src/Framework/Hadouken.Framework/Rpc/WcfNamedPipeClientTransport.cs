using System;
using System.ServiceModel;
using Hadouken.Framework.Plugins;

namespace Hadouken.Framework.Rpc
{
    public class WcfNamedPipeClientTransport :IClientTransport
    {
        private readonly string _rpcHost;
        private readonly Lazy<IPluginManagerService> _rpcProxy;

        public WcfNamedPipeClientTransport(string rpcHost)
        {
            _rpcHost = rpcHost;
            _rpcProxy = new Lazy<IPluginManagerService>(BuildProxy);
        }

        private IPluginManagerService BuildProxy()
        {
            var binding = new NetNamedPipeBinding
            {
                MaxBufferPoolSize = 10485760,
                MaxBufferSize = 10485760,
                MaxConnections = 10,
                MaxReceivedMessageSize = 10485760
            };

            // Create proxy
            var factory = new ChannelFactory<IPluginManagerService>(binding, _rpcHost);
            return factory.CreateChannel();
        }

        public string Send(string data)
        {
            return _rpcProxy.Value.RpcAsync(data).Result;
        }
    }
}
