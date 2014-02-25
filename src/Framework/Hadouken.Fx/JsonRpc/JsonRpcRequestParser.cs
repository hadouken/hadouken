namespace Hadouken.Fx.JsonRpc
{
    public class JsonRpcRequestParser : IJsonRpcRequestParser
    {
        public JsonRpcRequest Parse(string json)
        {
            return SimpleJson.DeserializeObject<JsonRpcRequest>(json);
        }
    }
}