using System.Collections.Generic;

namespace Hadouken.Fx.JsonRpc
{
    public class JsonRpcClient : IJsonRpcClient
    {
        private readonly IClientTransport _transport;
        private readonly IJsonSerializer _jsonSerializer;

        public JsonRpcClient(IClientTransport transport, IJsonSerializer jsonSerializer)
        {
            _transport = transport;
            _jsonSerializer = jsonSerializer;
        }

        public T Call<T>(string method, object parameters)
        {
            var request = new JsonRpcRequest
            {
                Id = 1,
                MethodName = method,
                Parameters = parameters
            };

            var json = _jsonSerializer.Serialize(request);
            var responseJson = _transport.Call(json);
            var response = (IDictionary<string, object>) _jsonSerializer.Deserialize(responseJson);

            if (!response.ContainsKey("result"))
            {
                return default(T);
            }
            
            var resultJson = _jsonSerializer.Serialize(response["result"]);
            return _jsonSerializer.Deserialize<T>(resultJson);
        }
    }
}