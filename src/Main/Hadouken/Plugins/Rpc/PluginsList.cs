using System;
using System.Linq;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins.Rpc
{
    [RpcMethod("plugins.list")]
    public class PluginsList : IRpcMethod
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsList(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public PluginListDto[] Execute()
        {
            return _pluginEngine.GetAll().Select(pm => new PluginListDto
                {
                    Name = pm.Name,
                    Version = pm.Version
                }).ToArray();
        }
    }

    public class PluginListDto
    {
        public string Name { get; set; }

        public Version Version { get; set; }
    }
}
