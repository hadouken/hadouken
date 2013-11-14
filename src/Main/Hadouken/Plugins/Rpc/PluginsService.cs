using System;
using System.Linq;

using Hadouken.Framework.Rpc;
using Hadouken.Framework.SemVer;
using NLog;

namespace Hadouken.Plugins.Rpc
{
    public class PluginsService : IJsonRpcService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IPluginEngine _pluginEngine;

        public PluginsService(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        [JsonRpcMethod("plugins.load")]
        public bool Load(string name)
        {
            Logger.Trace("Trying to load plugin {0}", name);

            try
            {
                _pluginEngine.LoadAsync(name);
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
                _pluginEngine.UnloadAsync(name);
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
                    Name = plugin.Manifest.Name,
                    Version = plugin.Manifest.Version,
                    State = plugin.State,
                    MemoryUsage = plugin.GetMemoryUsage(),
                }).ToArray();
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
