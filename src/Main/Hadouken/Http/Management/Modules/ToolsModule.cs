using System.Linq;
using System.Text;
using Hadouken.Plugins;
using NLog;
using NLog.Targets;

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

                var target = LogManager.Configuration.AllTargets.FirstOrDefault(t => t.Name == "memory");
                if (target != null)
                {
                    var memoryTarget = target as MemoryTarget;
                    if (memoryTarget != null)
                    {
                        foreach (var row in memoryTarget.Logs)
                        {
                            sb.AppendLine(row);
                        }
                    }
                }

                return View["Log", new {Log = sb.ToString()}];
            };
        }
    }
}
