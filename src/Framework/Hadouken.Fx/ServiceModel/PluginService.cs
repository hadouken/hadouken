using System.ServiceModel;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.Fx.ServiceModel
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PluginService : IPluginService
    {
        private readonly IRequestHandler _requestHandler;

        public PluginService(IRequestHandler requestHandler)
        {
            _requestHandler = requestHandler;
        }

        public string HandleJsonRpc(string request)
        {
            return _requestHandler.Handle(request);
        }
    }
}