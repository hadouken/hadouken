using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.HttpJsonRpc
{
    public class HttpJsonRpcPlugin : Plugin
    {
        private readonly IHttpJsonRpcServer _rpcServer;

        public HttpJsonRpcPlugin(IHttpJsonRpcServer rpcServer)
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
