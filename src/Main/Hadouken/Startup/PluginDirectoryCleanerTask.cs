using System;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;
using Serilog;

namespace Hadouken.Startup
{
    public sealed class PluginDirectoryCleanerTask : IStartupTask
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;

        public PluginDirectoryCleanerTask(ILogger logger, IConfiguration configuration, IFileSystem fileSystem)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");

            _logger = logger;
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

        public void Execute(string[] args)
        {
            var dir = _fileSystem.GetDirectory(_configuration.Plugins.BaseDirectory);

            foreach (var pluginDirectory in dir.Directories)
            {
                if (pluginDirectory.Files.Any(f => f.Name == Manifest.FileName)) continue;

                _logger.Information("Directory {Directory} did not have a manifest.json file. Removing it.",
                    pluginDirectory.FullPath);

                pluginDirectory.Delete(true);
            }
        }
    }
}