using Hadouken.Framework.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Rpc
{
    [RpcMethod("plugins.unload")]
    public class PluginsUnload : IRpcMethod
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsUnload(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public void Execute(string name)
        {
            var plugin = _pluginEngine.Get(name);

            if (plugin != null)
                plugin.Unload();
        }
    }
}
