using System;
using System.ServiceModel;
using System.Text;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Wcf;
using Nancy;

namespace Hadouken.Plugins.WebUI.Modules
{
    public class PluginsModule : NancyModule
    {
        public PluginsModule()
        {
            Get["/plugins/{id}/{path*}"] = _ => RequestFile(_.id.ToString(), _.path.ToString());
        }

        private string RequestFile(string pluginId, string path)
        {
            if (pluginId == "core.web")
                return null;

            var bindingFactory = new BindingFactory();
            var pluginEndpoint = new Uri("net.pipe://localhost/hdkn.plugins." + pluginId);
            var binding = bindingFactory.Create(pluginEndpoint);
            var proxy = new ChannelFactory<IPluginManagerService>(binding, pluginEndpoint.ToString());
            var channel = proxy.CreateChannel();

            var fileContents = channel.GetFileAsync(path).Result;

            if (fileContents == null)
                return null;

            return Encoding.UTF8.GetString(fileContents);
        }
    }
}
