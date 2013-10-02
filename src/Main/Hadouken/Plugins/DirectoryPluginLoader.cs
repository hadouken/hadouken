using System;
using System.IO;
using Hadouken.Configuration;
using Hadouken.IO;

namespace Hadouken.Plugins
{
    public class DirectoryPluginLoader : IPluginLoader
    {
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;

        public DirectoryPluginLoader(IConfiguration configuration, IFileSystem fileSystem)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

        public bool CanLoad(string path)
        {
            return _fileSystem.IsDirectory(path);
        }

        public IPluginManager Load(string path)
        {
            var manifestPath = Path.Combine(path, "manifest.json");

            if (!_fileSystem.FileExists(manifestPath))
                return null;

            return new PluginManager(path, _fileSystem);
        }
    }
}
