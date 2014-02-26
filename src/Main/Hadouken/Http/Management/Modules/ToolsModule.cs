using System.Linq;
using System.ServiceModel;
using Hadouken.Fx.ServiceModel;
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

            Post["/jsonrpc-debugger"] = _ =>
            {
                var plugin = Request.Form.pluginId;
                var json = Request.Form.json;
                var binding = new NetNamedPipeBinding();
                var endpoint = new EndpointAddress(string.Format("net.pipe://localhost/hadouken.plugins.{0}", plugin));

                using (var factory = new ChannelFactory<IPluginService>(binding, endpoint))
                {
                    var channel = factory.CreateChannel();
                    return channel.HandleJsonRpc(json);
                }
            };
        }
    }
}
