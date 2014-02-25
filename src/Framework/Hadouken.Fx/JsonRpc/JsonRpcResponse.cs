namespace Hadouken.Fx.JsonRpc
{
    public abstract class JsonRpcResponse
    {
        protected JsonRpcResponse()
        {
            ProtocolVersion = "2.0";
        }

        public object Id { get; protected set; }

        public string ProtocolVersion { get; private set; }
    }
}
