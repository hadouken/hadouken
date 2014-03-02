using System.Collections.Generic;

namespace Hadouken.Fx.JsonRpc
{
    public class JsonRpcRequestParser : IJsonRpcRequestParser
    {
        private readonly IJsonSerializer _serializer;

        public JsonRpcRequestParser(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public JsonRpcRequest Parse(string json)
        {
            var obj = (IDictionary<string, object>) _serializer.Deserialize(json);

            return new JsonRpcRequest
            {
                Id = obj["id"],
                ProtocolVersion = (string) obj["jsonrpc"],
                MethodName = (string) obj["method"],
                Parameters = obj["params"]
            };
        }
    }
}