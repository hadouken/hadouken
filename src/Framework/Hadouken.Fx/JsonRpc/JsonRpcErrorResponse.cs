namespace Hadouken.Fx.JsonRpc
{
    public class JsonRpcErrorResponse : JsonRpcResponse
    {
        public JsonRpcErrorResponse(object id)
        {
            Id = id;
        }

        public object Error { get; private set; }
    }
}