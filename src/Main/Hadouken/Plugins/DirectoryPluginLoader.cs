using System;
using System.IO;
using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.IO;
using Hadouken.Plugins.Metadata;

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

            using (var manifestJson = _fileSystem.OpenRead(manifestPath))
            {
                IManifest manifest;

                if (Manifest.TryParse(manifestJson, out manifest))
                {
                    var bootConfig = new BootConfig
                    {
                        DataPath = _configuration.ApplicationDataPath,
                        HostBinding = _configuration.Http.HostBinding,
                        Port = _configuration.Http.Port
                    };

                    return new PluginManager(path, manifest, _fileSystem, bootConfig);
                }
            }

            return null;
        }
    }
}
