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

        public object Call(string method)
        {
            return Call(method, null);
        }

        public object Call(string method, object parameters)
        {
            var resultJson = CallInternal(method, parameters);
            if (resultJson == null)
            {
                return null;
            }

            return _jsonSerializer.Deserialize(resultJson);
        }

        public T Call<T>(string method)
        {
            return Call<T>(method, null);
        }

        public T Call<T>(string method, object parameters)
        {
            var resultJson = CallInternal(method, parameters);
            if (resultJson == null)
            {
                return default(T);
            }

            return _jsonSerializer.Deserialize<T>(resultJson);
        }

        private string CallInternal(string method, object parameters)
        {
            var request = new JsonRpcRequest
            {
                Id = 1,
                MethodName = method,
                Parameters = parameters
            };

            var json = _jsonSerializer.Serialize(request);
            var responseJson = _transport.Call(json);
            var response = (IDictionary<string, object>)_jsonSerializer.Deserialize(responseJson);

            if (!response.ContainsKey("result"))
            {
                return null;
            }

            return _jsonSerializer.Serialize(response["result"]);
        }
    }
}