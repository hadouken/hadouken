using System.Runtime.Serialization;

namespace Hadouken.Fx.JsonRpc
{
    [DataContract]
    public class JsonRpcSuccessResponse : JsonRpcResponse
    {
        public JsonRpcSuccessResponse(object id, object result)
        {
            Id = id;
            Result = result;
        }

        [DataMember(Name = "result")]
        public object Result { get; private set; }
    }
}