using System.Linq;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public class ToolsModule : NancyModule
    {
        public ToolsModule(IPluginEngine pluginEngine)
            : base("tools")
        {
            this.RequiresAuthentication();

            Get["/jsonrpc-debugger"] = _ =>
            {
                var pluginIds = pluginEngine.GetAll().Select(p => p.Manifest.Name).ToArray();
                return View["JsonRpcDebugger", new {Plugins = pluginIds}];
            };
        }
    }
}
