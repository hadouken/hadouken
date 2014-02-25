using System.Linq;
using Hadouken.Configuration;
using Hadouken.Http.Management.Models;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public class PluginsModule : NancyModule
    {
        public PluginsModule(IPluginEngine pluginEngine, IPackageFactory packageFactory, IPackageInstaller packageInstaller)
            : base("plugins")
        {
            this.RequiresAuthentication();

            Get["/"] = _ =>
            {
                var type = Request.Query.t;
                var message = Request.Query.msg;

                if (type == "error")
                    type = "danger";

                var plugins = pluginEngine.GetAll();
                var dto = (from plugin in plugins
                    select new PluginListItem
                    {
                        Name = plugin.Manifest.Name,
                        StateMessage = (plugin.State == PluginState.Error ? "Error: " + plugin.ErrorMessage : plugin.State.ToString()),
                        Version = plugin.Manifest.Version
                    }).ToList();

                return
                    View[
                        "Index",
                        new
                        {
                            HasPlugins = dto.Any(),
                            Plugins = dto,
                            HasAlert = !string.IsNullOrEmpty(message),
                            AlertMessage = message,
                            AlertClass = type
                        }];
            };

            Get["/details/{id}"] = _ =>
            {
                IPluginManager plugin = pluginEngine.Get(_.id);

                if (plugin == null)
                {
                    return 404;
                }

                var dto = new PluginDetailsItem
                {
                    Name = plugin.Manifest.Name,
                    Description = "got this from the internet"
                };

                return View["Details", dto];
            };

            Get["/upload"] = _ => View["Upload"];

            Post["/upload"] = _ =>
            {
                if (!Request.Files.Any())
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=no-package");
                }

                var postedFile = Request.Files.First();
                var package = packageFactory.ReadFrom(postedFile.Value);

                if (package == null)
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=invalid-package");
                }

                var existingPlugin = pluginEngine.Get(package.Manifest.Name);

                if (existingPlugin != null)
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=package-already-exists");
                }

                // Save package to disk
                packageInstaller.Install(package);
                pluginEngine.Scan();

                return Response.AsRedirect("/manage/plugins?t=success&msg=package-uploaded");
            };
        }
    }
}
