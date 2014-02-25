using Hadouken.Fx.JsonRpc;

namespace Hadouken.Fx.Tests.JsonRpc
{
    internal class TestService : IJsonRpcService
    {
        [JsonRpcMethod("int32.noParams")]
        public int Int32_NoParams()
        {
            return 42;
        }
    }
}
