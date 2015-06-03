using System.Runtime.Serialization;

namespace Hadouken.Core.JsonRpc {
    [DataContract]
    public abstract class JsonRpcResponse {
        protected JsonRpcResponse() {
            this.ProtocolVersion = "2.0";
        }

        [DataMember(Name = "id")]
        public object Id { get; protected set; }

        [DataMember(Name = "jsonrpc")]
        public string ProtocolVersion { get; private set; }
    }
}