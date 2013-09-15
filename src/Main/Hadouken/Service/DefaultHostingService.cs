using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins;

namespace Hadouken.Service
{
    public class DefaultHostingService : HostingService
    {
        private readonly IPluginEngine _pluginEngine;
        private readonly IJsonRpcServer _rpcServer;

        public DefaultHostingService(IPluginEngine pluginEngine, IJsonRpcServer rpcServer)
        {
            _pluginEngine = pluginEngine;
            _rpcServer = rpcServer;
        }

        protected override void OnStart(string[] args)
        {
            _pluginEngine.Load();
            _rpcServer.Open();
        }

        protected override void OnStop()
        {
            _rpcServer.Close();
            _pluginEngine.Unload();
        }
    }
}
