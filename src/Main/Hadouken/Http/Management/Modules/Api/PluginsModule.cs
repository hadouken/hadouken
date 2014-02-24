using Hadouken.Plugins;
using Nancy;

namespace Hadouken.Http.Management.Modules.Api
{
    public class PluginsModule : NancyModule
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsModule(IPluginEngine pluginEngine)
            : base("api/plugins")
        {
            _pluginEngine = pluginEngine;
            Get["/"] = _ => "List of plugins";
        }
    }
}
