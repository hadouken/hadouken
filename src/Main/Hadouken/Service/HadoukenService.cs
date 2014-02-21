using System;
using Hadouken.Configuration;
using Hadouken.Events;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Wcf;
using Hadouken.Plugins;
using NLog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

	    private readonly IEventServer _eventServer;
	    private readonly IPluginEngine _pluginEngine;
	    private readonly IJsonRpcClient _rpcClient;
	    private readonly IServiceHost _gatewayHost;

        public HadoukenService(IConfiguration configuration,
            IEventServer eventServer,
            IPluginEngine pluginEngine,
            IJsonRpcClient rpcClient,
            IServiceHostFactory<IPluginManagerService> serviceHostFactory)
		{
		    _eventServer = eventServer;
		    _pluginEngine = pluginEngine;
            _rpcClient = rpcClient;
            _gatewayHost = serviceHostFactory.Create(new Uri(configuration.Rpc.GatewayUri));
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    _eventServer.Open();

		    _gatewayHost.Open();

		    _pluginEngine.LoadAll();
		}

		public void Stop()
		{
			Logger.Info("Stopping Hadouken");

		    _rpcClient.Call<bool>("events.publish", new object[] {"sys.unloading", ""});

		    _pluginEngine.UnloadAll();

		    _gatewayHost.Close();

		    _eventServer.Close();
		}
	}
}
