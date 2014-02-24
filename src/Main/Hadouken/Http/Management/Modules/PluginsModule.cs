using System.IO;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Framework.IO;
using Hadouken.Http.Management.Models;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public class PluginsModule : NancyModule
    {
        public PluginsModule(IConfiguration configuration, IFileSystem fileSystem, IPluginEngine pluginEngine)
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
                        Name = plugin.Package.Manifest.Name,
                        StateMessage = (plugin.State == PluginState.Error ? "Error: " + plugin.ErrorMessage : plugin.State.ToString()),
                        Version = plugin.Package.Manifest.Version
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
                    Name = plugin.Package.Manifest.Name,
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
                byte[] data;

                using (var ms = new MemoryStream())
                {
                    postedFile.Value.CopyTo(ms);
                    data = ms.ToArray();
                }

                var memFile = new InMemoryFile(() => new MemoryStream(data));
                IPackage package;

                if (!Package.TryParse(memFile, out package))
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=invalid-package");                    
                }

                var existingPlugin = pluginEngine.Get(package.Manifest.Name);

                if (existingPlugin != null)
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=package-already-exists");
                }

                // Save package to disk
                var fileName = string.Format("{0}-{1}.zip", package.Manifest.Name, package.Manifest.Version);
                var path = Path.Combine(configuration.Plugins.BaseDirectory, fileName);
                var file = fileSystem.GetFile(path);

                using (var stream = file.OpenWrite())
                {
                    stream.Write(data, 0, data.Length);
                }

                pluginEngine.Rebuild();

                return Response.AsRedirect("/manage/plugins?t=success&msg=package-uploaded");
            };
        }
    }
}
