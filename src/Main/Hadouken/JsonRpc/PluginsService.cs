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
            _pluginEngine = pluginEngine;
        }

        [JsonRpcMethod("core.plugins.list")]
        public string[] ListPlugins()
        {
            return _pluginEngine.GetAll().Select(p => p.Manifest.Name).ToArray();
        }
    }
}
