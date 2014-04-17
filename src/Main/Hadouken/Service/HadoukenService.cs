using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Fx.ServiceModel;
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
	    private readonly IPluginServiceHost _rpcServiceHost;
	    private readonly IHttpBackendServer _backendServer;
	    private readonly IApiConnection _apiConnection;
	    private readonly IFileSystem _fileSystem;
	    private readonly IPackageReader _packageReader;
	    private readonly IPackageInstaller _packageInstaller;
	    private readonly IPluginEngine _pluginEngine;

        public HadoukenService(IConfiguration configuration,
            IPluginServiceHost rpcServiceHost,
            IHttpBackendServer backendServer,
            IApiConnection apiConnection,
            IFileSystem fileSystem,
            IPackageReader packageReader,
            IPackageInstaller packageInstaller,
            IPluginEngine pluginEngine)
		{
            _configuration = configuration;
            _rpcServiceHost = rpcServiceHost;
            _backendServer = backendServer;
            _apiConnection = apiConnection;
            _fileSystem = fileSystem;
            _packageReader = packageReader;
            _packageInstaller = packageInstaller;
            _pluginEngine = pluginEngine;
		}

		public void Start(string[] args)
		{
			Logger.Info("Starting Hadouken");

		    _backendServer.Start();
		    _rpcServiceHost.Open();

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

		    _rpcServiceHost.Close();
		    _backendServer.Stop();
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
	            using (var ms = new MemoryStream(data))
	            {
	                var package = _packageReader.Read(ms);

	                if (package == null)
	                {
                        Logger.Error("Could not parse core package from {0}", pluginUri);
	                    continue;
	                }

	                _packageInstaller.Install(package);
                    Logger.Info("Core package {0} downloaded successfully.", package.Manifest.Name);
	            }
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
