using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework.Rpc.Hosting
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WcfJsonRpcServer : IJsonRpcServer, IWcfJsonRpcServer
    {
        private readonly IJsonRpcHandler _rpcHandler;
        private readonly ServiceHost _serviceHost;

        public WcfJsonRpcServer(string listenUri, IJsonRpcHandler rpcHandler)
        {
            _rpcHandler = rpcHandler;

            _serviceHost = new ServiceHost(this);
            _serviceHost.AddServiceEndpoint(typeof(IWcfJsonRpcServer), new NetNamedPipeBinding(), listenUri);
        }

        public void Open()
        {
            _serviceHost.Open();
        }

        public void Close()
        {
            _serviceHost.Close();
        }

        public string Call(string json)
        {
            return _rpcHandler.HandleAsync(json).Result;
        }
    }
}
