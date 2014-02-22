using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Events;
using Hadouken.Framework.IO;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Wcf;
using Hadouken.Http;
using Hadouken.Plugins;
using NLog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

	    private readonly IConfiguration _configuration;
	    private readonly IApiConnection _apiConnection;
	    private readonly IFileSystem _fileSystem;
	    private readonly IEventServer _eventServer;
	    private readonly IPluginEngine _pluginEngine;
	    private readonly IJsonRpcClient _rpcClient;
	    private readonly IServiceHost _gatewayHost;

        public HadoukenService(IConfiguration configuration,
            IApiConnection apiConnection,
            IFileSystem fileSystem,
            IEventServer eventServer,
            IPluginEngine pluginEngine,
            IJsonRpcClient rpcClient,
            IServiceHostFactory<IPluginManagerService> serviceHostFactory)
		{
            _configuration = configuration;
            _apiConnection = apiConnection;
            _fileSystem = fileSystem;
            _eventServer = eventServer;
		    _pluginEngine = pluginEngine;
            _rpcClient = rpcClient;
            _gatewayHost = serviceHostFactory.Create(new Uri(configuration.Rpc.GatewayUri));
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    try
		    {
		        BootstrapCorePlugins();
                _pluginEngine.Rebuild();
		    }
		    catch (Exception e)
		    {
		        Logger.FatalException("Could not bootstrap the core plugins.", e);
		    }

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

	    private void BootstrapCorePlugins()
	    {
	        var configuredFilePath = Path.Combine(_configuration.ApplicationDataPath, "CONFIGURED");
	        var configuredFile = _fileSystem.GetFile(configuredFilePath);

	        if (configuredFile.Exists)
	        {
	            return;
	        }

	        var corePluginsUri = new Uri(_configuration.Plugins.RepositoryUri, "get-core");

	        Logger.Info("Bootstrapping core plugins from {0}.", corePluginsUri);

            IList<Uri> corePlugins = _apiConnection.GetAsync<List<Uri>>(corePluginsUri).Result;

	        if (corePlugins == null || !corePlugins.Any())
	        {
	            Logger.Error("Could not find any core plugins. Hadouken may not work correctly.");
	            return;
	        }

	        foreach (var pluginUri in corePlugins)
	        {
	            var data = _apiConnection.DownloadData(pluginUri);

                // Parse it as a package
	            IPackage package;

	            if (!Package.TryParse(new InMemoryFile(() => new MemoryStream(data)), out package))
	            {
	                Logger.Error("Could not parse core package from {0}", pluginUri);
	                continue;
	            }

	            var packageFileName = string.Format("{0}-{1}.zip", package.Manifest.Name, package.Manifest.Version);
	            var packageFile = _fileSystem.GetFile(Path.Combine(_configuration.Plugins.BaseDirectory, packageFileName));

	            using (var stream = packageFile.OpenWrite())
	            {
	                stream.Write(data, 0, data.Length);
	            }

	            Logger.Info("Core package {0} downloaded successfully.", package.Manifest.Name);
	        }
            
            // Create the file.
	        using (var stream = configuredFile.OpenWrite())
            using (var streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(DateTime.UtcNow.ToString());
            }
	    }
	}
}
