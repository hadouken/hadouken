using System.Linq;
using System.Text;
using Hadouken.Http.Management.Models;
using Hadouken.Logging;
using Hadouken.Plugins;

namespace Hadouken.Http.Management.Modules
{
    public class ToolsModule : ModuleBase
    {
        public ToolsModule(IInMemorySink memorySink, IPluginEngine pluginEngine)
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
                var events = (from le in memorySink.LogEvents
                    select new LogEntry
                    {
                        Timestamp = le.Timestamp,
                        Level = le.Level.ToString(),
                        Message = le.RenderMessage()
                    });

                return View["Log", new {Events = events}];
            };
        }
    }
}
