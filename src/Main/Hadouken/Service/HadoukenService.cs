using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins;
using NLog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private readonly IPluginEngine _pluginEngine;
		private readonly IJsonRpcServer _rpcServer;

		public HadoukenService(IPluginEngine pluginEngine, IJsonRpcServer rpcServer)
		{
			_pluginEngine = pluginEngine;
			_rpcServer = rpcServer;
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

			_rpcServer.Open();

			_pluginEngine.ScanAsync().Wait();
			_pluginEngine.LoadAllAsync().Wait();
		}

		public void Stop()
		{
			Logger.Info("Stopping Hadouken");

			_pluginEngine.UnloadAllAsync().Wait();

			_rpcServer.Close();
		}
	}
}
