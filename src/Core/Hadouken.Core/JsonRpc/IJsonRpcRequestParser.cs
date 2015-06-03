namespace Hadouken.Core.JsonRpc {
    public interface IJsonRpcRequestParser {
        JsonRpcRequest Parse(string json);
    }
}