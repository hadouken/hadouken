namespace Hadouken.Fx.JsonRpc
{
    public sealed class JsonRpcRequest
    {
        public object Id { get; set; }

        public string ProtocolVersion { get; set; }

        public string MethodName { get; set; }

        public object Parameters { get; set; }
    }
}
