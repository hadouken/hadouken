using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
namespace Hadouken.Plugins.Config
{
    public class ConfigPlugin : Plugin
    {
        private readonly IWcfJsonRpcServer _rpcServer;

        public ConfigPlugin(IWcfJsonRpcServer rpcServer)
        {
            _rpcServer = rpcServer;
        }

        public override void Load()
        {
            _rpcServer.Open();
        }

        public override void Unload()
        {
            _rpcServer.Close();
        }
    }
}
