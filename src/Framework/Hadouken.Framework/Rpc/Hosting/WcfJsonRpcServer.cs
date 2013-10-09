using System;
using System.ServiceModel;

namespace Hadouken.Framework.Rpc.Hosting
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfJson : IWcfJsonRpcServer
    {
        private readonly IJsonRpcHandler _handler;

        public WcfJson(IJsonRpcHandler handler)
        {
            _handler = handler;
        }

        public string Call(string json)
        {
            return _handler.HandleAsync(json).Result;
        }
    }

    public class WcfJsonRpcServer : IJsonRpcServer
    {
        private readonly ServiceHost _serviceHost;

        public WcfJsonRpcServer(ServiceHost serviceHost)
        {
            _serviceHost = serviceHost;
        }

        public void Open()
        {
            _serviceHost.Open();
        }

        public void Close()
        {
            _serviceHost.Close();
        }
    }
}
