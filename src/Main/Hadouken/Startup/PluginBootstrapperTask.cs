using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Http;
using Hadouken.Plugins;
using Serilog;

namespace Hadouken.Startup
{
    public class PluginBootstrapperTask : IStartupTask
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;
        private readonly IApiConnection _apiConnection;

        public PluginBootstrapperTask(ILogger logger,
            IConfiguration configuration,
            IFileSystem fileSystem,
            IApiConnection apiConnection)
        {
            _logger = logger;
            _configuration = configuration;
            _fileSystem = fileSystem;
            _apiConnection = apiConnection;
        }

        public void Execute(string[] args)
        {
            if (args != null && args.Contains("--no-http-bootstrap"))
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


            var corePluginsUri = new Uri(new Uri(_configuration.PluginRepositoryUrl), "get-core");

            _logger.Information("Bootstrapping core plugins from {Uri}.", corePluginsUri);

            var corePlugins = _apiConnection.GetAsync<List<Uri>>(corePluginsUri).Result;

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
                    var package = (object)null; //_packageReader.Read(ms);

                    if (package == null)
                    {
                        _logger.Error("Could not parse core package from {Uri}", pluginUri);
                        continue;
                    }

                    //_packageInstaller.Install(package);
                    //_logger.Information("Core package {0} downloaded successfully.", package.Manifest.Name);
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