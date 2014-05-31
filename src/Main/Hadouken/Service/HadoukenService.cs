using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Fx.ServiceModel;
using Hadouken.Http.Management;
using Hadouken.Plugins;
using Hadouken.Startup;
using Serilog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
	    private readonly ILogger _logger;
	    private readonly IEnumerable<IStartupTask> _startupTasks;
	    private readonly IPluginServiceHost _rpcServiceHost;
	    private readonly IHttpBackendServer _backendServer;
	    private readonly IPluginEngine _pluginEngine;

        public HadoukenService(ILogger logger,
            IEnumerable<IStartupTask> startupTasks,
            IPluginServiceHost rpcServiceHost,
            IHttpBackendServer backendServer,
            IPluginEngine pluginEngine)
		{
            _logger = logger;
            _startupTasks = startupTasks;
            _rpcServiceHost = rpcServiceHost;
            _backendServer = backendServer;
            _pluginEngine = pluginEngine;
		}

		public void Start(string[] args)
		{
            _logger.Information("Starting Hadouken.");

		    if (_startupTasks.Any())
		    {
		        _logger.Information("Running startup tasks.");

		        foreach (var task in _startupTasks)
		        {
		            try
		            {
		                _logger.Information("Executing {TaskName} with arguments: {Arguments}.", task.GetType().FullName, args);
		                task.Execute(args);
		            }
		            catch (Exception e)
		            {
		                _logger.Fatal(e, "Task failed to execute.");
		            }
		        }
		    }

		    _backendServer.Start();
		    _rpcServiceHost.Open();

            _pluginEngine.Refresh();
		    _pluginEngine.LoadAll();
		}

		public void Stop()
		{
            _logger.Information("Stopping Hadouken.");

		    _pluginEngine.UnloadAll();

		    _rpcServiceHost.Close();
		    _backendServer.Stop();
		}
	}
}
