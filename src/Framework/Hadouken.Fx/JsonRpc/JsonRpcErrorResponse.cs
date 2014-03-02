using System.Runtime.Serialization;

namespace Hadouken.Fx.JsonRpc
{
    [DataContract]
    public class JsonRpcErrorResponse : JsonRpcResponse
    {
        public JsonRpcErrorResponse(object id, int errorCode, string message)
        {
            Id = id;
            Error = new
            {
                code = errorCode,
                message = message,
            };
        }

        [DataMember(Name = "error")]
        public object Error { get; private set; }
    }
}