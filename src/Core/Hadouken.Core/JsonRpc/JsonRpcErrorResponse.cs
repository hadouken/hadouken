using System.Runtime.Serialization;

namespace Hadouken.Core.JsonRpc {
    [DataContract]
    public class JsonRpcErrorResponse : JsonRpcResponse {
        public JsonRpcErrorResponse(object id, int errorCode, string message, object data = null) {
            this.Id = id;
            this.Error = new ErrorObject(errorCode, message, data);
        }

        [DataMember(Name = "error")]
        public ErrorObject Error { get; private set; }
    }

    [DataContract]
    public class ErrorObject {
        public ErrorObject(int errorCode, string message, object data = null) {
            this.Code = errorCode;
            this.Message = message;
            this.Data = data;
        }

        [DataMember(Name = "code")]
        public int Code { get; private set; }

        [DataMember(Name = "message")]
        public string Message { get; private set; }

        [DataMember(Name = "data")]
        public object Data { get; private set; }
    }
}