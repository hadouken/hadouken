using System;
using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Framework.IO;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.SemVer;
using NLog;

namespace Hadouken.Plugins.Rpc
{
    public class PluginsService : IJsonRpcService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPluginEngine _pluginEngine;
        private readonly IConfiguration _configuration;
        private readonly IFileSystem _fileSystem;

        public PluginsService(IPluginEngine pluginEngine, IConfiguration configuration, IFileSystem fileSystem)
        {
            _pluginEngine = pluginEngine;
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

        [JsonRpcMethod("plugins.load")]
        public bool Load(string name)
        {
            Logger.Trace("Trying to load plugin {0}", name);

            try
            {
                _pluginEngine.Load(name);
                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorException(String.Format("Could not load plugin {0}", name), e);
                return false;
            }
        }

        [JsonRpcMethod("plugins.unload")]
        public bool Unload(string name)
        {
            Logger.Trace("Trying to unload plugin {0}", name);

            try
            {
                _pluginEngine.Unload(name);
                return true;
            }
            catch (Exception e)
            {
                Logger.ErrorException(String.Format("Could not unload plugin {0}", name), e);
                return false;
            }
        }

        [JsonRpcMethod("plugins.list")]
        public PluginDto[] List()
        {
            var plugins = _pluginEngine.GetAll();

            return (from plugin in plugins
                select new PluginDto()
                {
                    Name = plugin.Package.Manifest.Name,
                    Version = plugin.Package.Manifest.Version,
                    State = plugin.State,
                    MemoryUsage = plugin.GetMemoryUsage(),
                }).ToArray();
        }

        [JsonRpcMethod("plugins.getFileContents")]
        public byte[] GetFileContents(string pluginId, string fileName)
        {
            // Get the plugin
            var plugin = _pluginEngine.Get(pluginId);

            if (plugin == null)
                return null;

            var file = plugin.Package.GetFile(fileName);

            if (file == null)
                return null;

            using (var stream = file.OpenRead())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }

        [JsonRpcMethod("plugins.installFromFile")]
        public bool Upload(byte[] packageData, string password)
        {
            var file = new InMemoryFile(() => new MemoryStream(packageData));
            
            IPackage package;

            if (!Package.TryParse(file, out package))
            {
                return false;
            }

            _pluginEngine.InstallOrUpgrade(package);

            return true;
        }

        [JsonRpcMethod("plugins.install")]
        public bool Install(string packageUrl, string password)
        {
            if (String.IsNullOrEmpty(packageUrl))
                return false;

            // Validate password

            // 

            throw new NotImplementedException();
        }
    }

    public class PluginDto
    {
        public string Name { get; set; }

        public SemanticVersion Version { get; set; }

        public long MemoryUsage { get; set; }

        public PluginState  State { get; set; }
    }
}
