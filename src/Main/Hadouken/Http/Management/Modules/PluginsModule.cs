using System.Linq;
using Hadouken.Http.Management.Models;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public class PluginsModule : NancyModule
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsModule(IPluginEngine pluginEngine)
            : base("plugins")
        {
            _pluginEngine = pluginEngine;
            this.RequiresAuthentication();

            Get["/"] = _ =>
            {
                var plugins = _pluginEngine.GetAll();
                var dto = (from plugin in plugins
                    select new PluginListItem
                    {
                        Name = plugin.Package.Manifest.Name,
                        Version = plugin.Package.Manifest.Version
                    }).ToList();

                return View["Index", new{HasPlugins = dto.Any(),Plugins = dto}];
            };

            Get["/details/{id}"] = _ =>
            {
                IPluginManager plugin = _pluginEngine.Get(_.id);

                if (plugin == null)
                {
                    return 404;
                }

                var dto = new PluginDetailsItem
                {
                    Name = plugin.Package.Manifest.Name,
                    Description = "got this from the internet"
                };

                return View["Details", dto];
            };
        }
    }
}
