using System.ServiceModel;

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

            var binding = new NetNamedPipeBinding
                {
                    MaxBufferPoolSize = 10485760,
                    MaxBufferSize = 10485760,
                    MaxConnections = 10,
                    MaxReceivedMessageSize = 10485760
                };

            _serviceHost.AddServiceEndpoint(typeof(IWcfJsonRpcServer), binding, listenUri);
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
