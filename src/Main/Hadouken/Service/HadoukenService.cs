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
using Serilog;

namespace Hadouken.Service
{
	public class HadoukenService : IHadoukenService
	{
	    private readonly ILogger _logger;
	    private readonly IConfiguration _configuration;
	    private readonly IPluginServiceHost _rpcServiceHost;
	    private readonly IHttpBackendServer _backendServer;
	    private readonly IApiConnection _apiConnection;
	    private readonly IFileSystem _fileSystem;
	    private readonly IPackageReader _packageReader;
	    private readonly IPackageInstaller _packageInstaller;
	    private readonly IPluginEngine _pluginEngine;

        public HadoukenService(ILogger logger,
            IConfiguration configuration,
            IPluginServiceHost rpcServiceHost,
            IHttpBackendServer backendServer,
            IApiConnection apiConnection,
            IFileSystem fileSystem,
            IPackageReader packageReader,
            IPackageInstaller packageInstaller,
            IPluginEngine pluginEngine)
		{
            _logger = logger;
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
            _logger.Information("Starting Hadouken");

		    _backendServer.Start();
		    _rpcServiceHost.Open();

		    try
		    {
		        BootstrapCorePlugins(args);
		    }
		    catch (Exception e)
		    {
                _logger.Fatal(e, "Could not bootstrap the core plugins.");
		    }

            _pluginEngine.Scan();
		    _pluginEngine.LoadAll();
		}

		public void Stop()
		{
            _logger.Information("Stopping Hadouken");

		    _pluginEngine.UnloadAll();

		    _rpcServiceHost.Close();
		    _backendServer.Stop();
		}

	    private void BootstrapCorePlugins(string[] args)
	    {
	        if (args != null && args.Any(s => s == "--no-http-bootstrap"))
	        {
                _logger.Warning("HTTP bootstrapping disabled. No plugins will be downloaded.");
	            return;
	        }

	        var configuredFilePath = Path.Combine(_configuration.ApplicationDataPath, "CONFIGURED");
	        var configuredFile = _fileSystem.GetFile(configuredFilePath);

	        if (configuredFile.Exists)
	        {
	            return;
	        }

	        var corePluginsUri = new Uri(_configuration.Plugins.RepositoryUri, "get-core");

            _logger.Information("Bootstrapping core plugins from {Uri}.", corePluginsUri);

            IList<Uri> corePlugins = _apiConnection.GetAsync<List<Uri>>(corePluginsUri).Result;

	        if (corePlugins == null || !corePlugins.Any())
	        {
                _logger.Error("Could not find any core plugins. Hadouken may not work correctly.");
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
                        _logger.Error("Could not parse core package from {Uri}", pluginUri);
	                    continue;
	                }

	                _packageInstaller.Install(package);
                    _logger.Information("Core package {0} downloaded successfully.", package.Manifest.Name);
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
