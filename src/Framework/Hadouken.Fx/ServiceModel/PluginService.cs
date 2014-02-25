using System.ServiceModel;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.Fx.ServiceModel
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class PluginService : IPluginService
    {
        private readonly IRequestHandler _requestHandler;
        private readonly IJsonRpcRequestParser _requestParser;
        private readonly IJsonSerializer _jsonSerializer;

        public PluginService(IRequestHandler requestHandler, IJsonRpcRequestParser requestParser, IJsonSerializer jsonSerializer)
        {
            _requestHandler = requestHandler;
            _requestParser = requestParser;
            _jsonSerializer = jsonSerializer;
        }

        public string HandleJsonRpc(string request)
        {
            var parsedRequest = _requestParser.Parse(request);
            var response = _requestHandler.Handle(parsedRequest);

            return _jsonSerializer.Serialize(response);
        }
    }
}