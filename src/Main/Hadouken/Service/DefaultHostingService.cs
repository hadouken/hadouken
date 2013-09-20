using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins;
using NLog;

namespace Hadouken.Service
{
    public class DefaultHostingService : HostingService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPluginEngine _pluginEngine;
        private readonly IJsonRpcServer _rpcServer;

        public DefaultHostingService(IPluginEngine pluginEngine, IJsonRpcServer rpcServer)
        {
            _pluginEngine = pluginEngine;
            _rpcServer = rpcServer;
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Starting Hadouken");

            _pluginEngine.Load();
            _rpcServer.Open();
        }

        protected override void OnStop()
        {
            Logger.Info("Stopping Hadouken");

            _rpcServer.Close();
            _pluginEngine.Unload();
        }
    }
}
