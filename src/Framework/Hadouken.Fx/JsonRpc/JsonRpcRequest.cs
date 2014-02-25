using System.Runtime.Serialization;

namespace Hadouken.Fx.JsonRpc
{
    public sealed class JsonRpcRequest
    {
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
