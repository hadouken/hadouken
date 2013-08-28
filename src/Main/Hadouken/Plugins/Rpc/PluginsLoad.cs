using Hadouken.Framework.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Plugins.Rpc
{
    [RpcMethod("plugins.load")]
    public class PluginsLoad : IRpcMethod
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsLoad(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public void Execute(string name)
        {
            var plugin = _pluginEngine.Get(name);

            if (plugin != null)
                plugin.Load();
        }
    }
}
