using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Http;
using Hadouken.Http.Management;
using Hadouken.Plugins;
using NLog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

	    private readonly IConfiguration _configuration;
	    private readonly IManagementServer _managementServer;
	    private readonly IApiConnection _apiConnection;
	    private readonly IFileSystem _fileSystem;
	    private readonly IPluginEngine _pluginEngine;

        public HadoukenService(IConfiguration configuration,
            IManagementServer managementServer,
            IApiConnection apiConnection,
            IFileSystem fileSystem,
            IPluginEngine pluginEngine)
		{
            _configuration = configuration;
            _managementServer = managementServer;
            _apiConnection = apiConnection;
            _fileSystem = fileSystem;
		    _pluginEngine = pluginEngine;
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    _managementServer.Start();

		    try
		    {
		        BootstrapCorePlugins(args);
		    }
		    catch (Exception e)
		    {
		        Logger.FatalException("Could not bootstrap the core plugins.", e);
		    }

            _pluginEngine.Scan();
		    _pluginEngine.LoadAll();
		}

		public void Stop()
		{
			Logger.Info("Stopping Hadouken");

		    _pluginEngine.UnloadAll();

		    _managementServer.Stop();
		}

	    private void BootstrapCorePlugins(string[] args)
	    {
	        if (args != null && args.Any(s => s == "--no-http-bootstrap"))
	        {
	            Logger.Warn("HTTP bootstrapping disabled. No plugins will be downloaded.");
	            return;
	        }

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
	            if (!Package.TryParse(new MemoryStream(data), out package))
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
