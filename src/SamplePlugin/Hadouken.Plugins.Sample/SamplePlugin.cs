using Hadouken.Fx;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.Plugins.Sample
{
    public class SamplePlugin : Plugin
    {
        private readonly IJsonRpcClient _rpcClient;

        public SamplePlugin(PluginConfiguration configuration, IJsonRpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        public override void Load()
        {
            _rpcClient.Call("core.logging.log",
                new []{"Foo.Hadouken", "Debug", "Zip fooooo's?!"});
        }
    }
}
