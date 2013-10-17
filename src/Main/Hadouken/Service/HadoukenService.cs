using System;
using Hadouken.Configuration;
using Hadouken.Events;
using Hadouken.Framework.Plugins;
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
	    private readonly IServiceHost _gatewayHost;

        public HadoukenService(IConfiguration configuration,
            IEventServer eventServer,
            IPluginEngine pluginEngine,
            IServiceHostFactory<IPluginManagerService> serviceHostFactory)
		{
		    _eventServer = eventServer;
		    _pluginEngine = pluginEngine;
            _gatewayHost = serviceHostFactory.Create(new Uri(configuration.Rpc.GatewayUri));
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    _eventServer.Open();

		    _gatewayHost.Open();

			_pluginEngine.ScanAsync().Wait();
			_pluginEngine.LoadAllAsync().Wait();
		}

		public void Stop()
		{
			Logger.Info("Stopping Hadouken");

			_pluginEngine.UnloadAllAsync().Wait();

		    _gatewayHost.Close();

		    _eventServer.Close();
		}
	}
}
