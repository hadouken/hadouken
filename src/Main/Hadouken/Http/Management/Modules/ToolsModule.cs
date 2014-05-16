using System.Linq;
using System.Text;
using Hadouken.Plugins;

namespace Hadouken.Http.Management.Modules
{
    public class ToolsModule : ModuleBase
    {
        public ToolsModule(IPluginEngine pluginEngine)
            : base("tools")
        {
            Get["/events"] = _ => View["Events"];

            Get["/jsonrpc-debugger"] = _ =>
            {
                var pluginIds = pluginEngine.GetAll().Select(p => p.Manifest.Name).ToArray();
                return View["JsonRpcDebugger", new {Plugins = pluginIds}];
            };

            Get["/log"] = _ =>
            {
                var sb = new StringBuilder();

                return View["Log", new {Log = sb.ToString()}];
            };
        }
    }
}
