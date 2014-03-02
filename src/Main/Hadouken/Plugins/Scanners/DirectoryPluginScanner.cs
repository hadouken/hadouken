using System.Collections.Generic;
using System.Linq;
using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins.Scanners
{
    public class DirectoryPluginScanner : IPluginScanner
    {
        private readonly string _pluginDirectory;
        private readonly IFileSystem _fileSystem;
        private readonly IPluginManagerFactory _managerFactory;

        public DirectoryPluginScanner(string pluginDirectory, IFileSystem fileSystem, IPluginManagerFactory managerFactory)
        {
            _pluginDirectory = pluginDirectory;
            _fileSystem = fileSystem;
            _managerFactory = managerFactory;
        }

        public IEnumerable<IPluginManager> Scan()
        {
            var result = new List<IPluginManager>();

            var baseDirectory = _fileSystem.GetDirectory(_pluginDirectory);
            var pluginDirectories = baseDirectory.Directories;

            foreach (var directory in pluginDirectories)
            {
                // Find manifest file
                var manifestFile = directory.Files.SingleOrDefault(f => f.Name == Manifest.FileName);
                if (manifestFile == null || !manifestFile.Exists)
                {
                    continue;
                }

                using (var manifestStream = manifestFile.OpenRead())
                {
                    IManifest manifest;
                    if (!Manifest.TryParse(manifestStream, out manifest))
                    {
                        continue;
                    }

                    var manager = _managerFactory.Create(directory, manifest);
                    result.Add(manager);
                }
            }

            return result;
        }
    }
}
