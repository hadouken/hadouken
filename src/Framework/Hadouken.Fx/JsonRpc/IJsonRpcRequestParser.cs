namespace Hadouken.Fx.JsonRpc
{
    public interface IJsonRpcRequestParser
    {
        JsonRpcRequest Parse(string json);
    }
}
