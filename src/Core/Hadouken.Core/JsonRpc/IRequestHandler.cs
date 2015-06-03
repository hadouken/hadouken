namespace Hadouken.Core.JsonRpc {
    public interface IRequestHandler {
        JsonRpcResponse Handle(JsonRpcRequest request);
    }
}