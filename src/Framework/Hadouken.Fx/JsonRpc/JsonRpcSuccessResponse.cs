namespace Hadouken.Fx.JsonRpc
{
    public class JsonRpcSuccessResponse : JsonRpcResponse
    {
        public JsonRpcSuccessResponse(object id, object result)
        {
            Id = id;
            Result = result;
        }

        public object Result { get; private set; }
    }
}