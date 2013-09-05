using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Wcf
{
    public class WcfClientTransport : IClientTransport
    {
        private readonly IWcfRpcHost _hostProxy;

        public WcfClientTransport(string endpoint)
        {
            _hostProxy = CreateHostProxy(endpoint);
        }

        public Task<string> Send(string data)
        {
            return Task.Run(() => _hostProxy.Call(data));
        }

        public void Dispose()
        {
        }

        private IWcfRpcHost CreateHostProxy(string endpoint)
        {
            var factory = new ChannelFactory<IWcfRpcHost>(new NetNamedPipeBinding(), endpoint);
            return factory.CreateChannel();
        }
    }
}
