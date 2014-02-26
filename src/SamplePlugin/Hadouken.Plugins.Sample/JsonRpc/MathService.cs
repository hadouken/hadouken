using Hadouken.Fx.JsonRpc;

namespace Hadouken.Plugins.Sample.JsonRpc
{
    public class MathService : IJsonRpcService
    {
        [JsonRpcMethod("math.add")]
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
