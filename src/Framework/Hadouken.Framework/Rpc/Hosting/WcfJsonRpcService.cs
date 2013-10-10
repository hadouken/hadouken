using System.ServiceModel;

namespace Hadouken.Framework.Rpc.Hosting
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class WcfJsonRpcService : IWcfRpcService
    {
        private readonly IJsonRpcHandler _handler;

        public WcfJsonRpcService(IJsonRpcHandler handler)
        {
            _handler = handler;
        }

        public string Call(string json)
        {
            return _handler.HandleAsync(json).Result;
        }
    }
}