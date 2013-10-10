using System;
using System.ServiceModel;

namespace Hadouken.Framework.Rpc.Hosting
{
    public interface IWcfJsonRpcServer : IJsonRpcServer
    {
    }

    public class WcfJsonRpcServer : IWcfJsonRpcServer
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
