using System;
using System.Linq;
using Hadouken.Fx.JsonRpc;
using Hadouken.Plugins;

namespace Hadouken.JsonRpc
{
    public class PluginsService : IJsonRpcService
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsService(IPluginEngine pluginEngine)
        {
            if (pluginEngine == null)
            {
                throw new ArgumentNullException("pluginEngine");
            }

            _pluginEngine = pluginEngine;
        }

        [JsonRpcMethod("core.plugins.list")]
        public string[] ListPlugins()
        {
            return _pluginEngine.GetAll().Select(p => p.Manifest.Name).ToArray();
        }

        [JsonRpcMethod("core.plugins.getVersion")]
        public string GetVersion(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);
            if (plugin == null)
            {
                return null;
            }

            return plugin.Manifest.Version.ToString();
        }
    }
}
