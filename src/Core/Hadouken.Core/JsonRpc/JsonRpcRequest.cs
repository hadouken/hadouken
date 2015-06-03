using System.Runtime.Serialization;

namespace Hadouken.Core.JsonRpc
{
    [DataContract]
    public sealed class JsonRpcRequest
    {
        public JsonRpcRequest()
        {
            this.ProtocolVersion = "2.0";
        }

        [DataMember(Name = "id")]
        public object Id { get; set; }

        [DataMember(Name = "jsonrpc")]
        public string ProtocolVersion { get; set; }

        [DataMember(Name = "method")]
        public string MethodName { get; set; }

        [DataMember(Name = "params")]
        public object Parameters { get; set; }
    }
}
