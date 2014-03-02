namespace Hadouken.Fx.JsonRpc
{
    public interface IRequestHandler
    {
        JsonRpcResponse Handle(JsonRpcRequest request);
    }
}
