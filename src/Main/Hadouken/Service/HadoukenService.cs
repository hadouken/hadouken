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
		private readonly IHttpJsonRpcServer _httpRpcServer;
	    private readonly WcfJsonRpcServer _wcfRpcServer;

	    public HadoukenService(IEventServer eventServer, IPluginEngine pluginEngine, IHttpJsonRpcServer rpcServer, WcfJsonRpcServer wcfRpcServer)
		{
		    _eventServer = eventServer;
		    _pluginEngine = pluginEngine;
			_httpRpcServer = rpcServer;
		    _wcfRpcServer = wcfRpcServer;
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    _eventServer.Open();

		    _wcfRpcServer.Open();
			_httpRpcServer.Open();

			_pluginEngine.ScanAsync().Wait();
			_pluginEngine.LoadAllAsync().Wait();
		}

		public void Stop()
		{
			Logger.Info("Stopping Hadouken");

			_pluginEngine.UnloadAllAsync().Wait();

			_httpRpcServer.Close();
		    _wcfRpcServer.Close();

		    _eventServer.Close();
		}
	}
}
