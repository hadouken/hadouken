using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.Rpc
{
    public class PluginsService : IJsonRpcService
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsService(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        [JsonRpcMethod("plugins.load")]
        public bool Load(string name)
        {
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
            catch (Exception)
            {
                return false;
            }
        }

        [JsonRpcMethod("plugins.unload")]
        public bool Unload(string name)
        {
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
            catch (Exception)
            {
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
                    Name = plugin.Name,
                    Version = plugin.Version,
                    State = plugin.State
                }).ToArray();
        }
    }

    public class PluginDto
    {
        public string Name { get; set; }

        public Version Version { get; set; }

        public PluginState  State { get; set; }
    }
}
