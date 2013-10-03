using Hadouken.Events;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins;
using NLog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

	    private readonly IEventServer _eventServer;
	    private readonly IPluginEngine _pluginEngine;
		private readonly IJsonRpcServer _rpcServer;

		public HadoukenService(IEventServer eventServer, IPluginEngine pluginEngine, IJsonRpcServer rpcServer)
		{
		    _eventServer = eventServer;
		    _pluginEngine = pluginEngine;
			_rpcServer = rpcServer;
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    _eventServer.Open();

			_rpcServer.Open();

			_pluginEngine.ScanAsync().Wait();
			_pluginEngine.LoadAllAsync().Wait();
		}

		public void Stop()
		{
			Logger.Info("Stopping Hadouken");

			_pluginEngine.UnloadAllAsync().Wait();

			_rpcServer.Close();

		    _eventServer.Close();
		}
	}
}
