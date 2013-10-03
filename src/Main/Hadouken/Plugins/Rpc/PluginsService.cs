using System;
using System.Linq;

using Hadouken.Framework.Rpc;
using Hadouken.Framework.SemVer;
using Hadouken.Plugins.Metadata;
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

            var plugin = _pluginEngine.Get(name);

            if (plugin == null)
                return false;

            if (plugin.State != PluginState.Unloaded)
                return false;

            try
            {
                plugin.Load();
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

            var plugin = _pluginEngine.Get(name);

            if (plugin == null)
                return false;

            if (plugin.State != PluginState.Loaded)
                return false;

            try
            {
                plugin.Unload();
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
                    State = plugin.State
                }).ToArray();
        }
    }

    public class PluginDto
    {
        public string Name { get; set; }

        public SemanticVersion Version { get; set; }

        public PluginState  State { get; set; }
    }
}
