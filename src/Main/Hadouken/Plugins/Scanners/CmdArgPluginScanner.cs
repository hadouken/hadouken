using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;
using Serilog;

namespace Hadouken.Plugins.Scanners
{
    public class CmdArgPluginScanner : IPluginScanner
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IPluginManagerFactory _managerFactory;

        public CmdArgPluginScanner(ILogger logger, IFileSystem fileSystem, IPluginManagerFactory managerFactory)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _managerFactory = managerFactory;
        }

        public IEnumerable<IPluginManager> Scan()
        {
            var args = Environment.GetCommandLineArgs();
            args = args.SkipWhile(s => s != "--plugin").ToArray();

            if (!args.Any()
                || args.Length < 2
                || args.First() != "--plugin")
            {
                return Enumerable.Empty<IPluginManager>();
            }

            var path = args.Skip(1).First();
            var directory = _fileSystem.GetDirectory(path);

            // Find manifest file
            var manifestFile = directory.Files.SingleOrDefault(f => f.Name == Manifest.FileName);
            if (manifestFile == null || !manifestFile.Exists)
            {
                return Enumerable.Empty<IPluginManager>();
            }

            using (var manifestStream = manifestFile.OpenRead())
            {
                IManifest manifest;
                if (!Manifest.TryParse(manifestStream, out manifest))
                {
                    return Enumerable.Empty<IPluginManager>();
                }

                try
                {
                    return new[] {_managerFactory.Create(directory, manifest)};
                }
                catch (PluginException e)
                {
                    _logger.Error(e, "Could not create plugin manager.");
                    return Enumerable.Empty<IPluginManager>();
                }
            }
        }
    }
}
