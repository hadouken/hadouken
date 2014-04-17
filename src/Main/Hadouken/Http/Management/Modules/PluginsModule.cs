using System.IO;
using System.Linq;
using Hadouken.Fx.JsonRpc;
using Hadouken.Http.Management.Models;
using Hadouken.Plugins;
using Nancy;
using System.Text;

namespace Hadouken.Http.Management.Modules
{
    public class PluginsModule : NancyModule
    {
        public PluginsModule(IPluginEngine pluginEngine, IPackageReader packageReader, IJsonRpcClient rpcClient)
            : base("plugins")
        {
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
                        StateMessage =
                            (plugin.State == PluginState.Error
                                ? "Error: " + plugin.ErrorMessage
                                : plugin.State.ToString()),
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
                    Path = plugin.BaseDirectory.FullPath,
                    Version = plugin.Manifest.Version
                };

                return View["Details", dto];
            };

            Get["/uninstall/{id}"] = _ =>
            {
                IPluginManager plugin = pluginEngine.Get(_.id);

                if (plugin == null)
                {
                    return 404;
                }

                string[] deps;
                var canUninstall = pluginEngine.CanUnload(_.id, out deps);

                return View["Uninstall", new {CanUninstall = canUninstall, Dependencies = deps, PluginId = _.id}];
            };

            Post["/uninstall"] = _ =>
            {
                string id = Request.Form.id;
                IPluginManager plugin = pluginEngine.Get(id);

                if (plugin == null)
                {
                    return 404;
                }

                pluginEngine.Unload(id);
                pluginEngine.Uninstall(id);

                return 200;
            };

            Get["/upload"] = _ => View["Upload"];

            Post["/upload"] = _ =>
            {
                if (!Request.Files.Any())
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=no-package");
                }

                var postedFile = Request.Files.First();
                var package = packageReader.Read(postedFile.Value);

                if (package == null)
                {
                    return Response.AsRedirect("/manage/plugins?t=error&msg=invalid-package");
                }

                pluginEngine.InstallOrUpgrade(package);

                return Response.AsRedirect("/manage/plugins?t=success&msg=package-uploaded");
            };

            Get["/settings/{id}"] = _ =>
            {
                string id = _.id;
                var plugin = pluginEngine.Get(id);
                if (plugin == null)
                {
                    return 404;
                }

                var templateData = rpcClient.Call<byte[]>(string.Format("{0}.config.template", id));
                if (templateData == null)
                {
                    return 404;
                }

                var template = Encoding.UTF8.GetString(templateData);

                return View["Settings", new {PluginId = _.id, Template = template}];
            };

            Get["/{id}/{path*}"] = _ =>
            {
                string id = _.id;
                string path = _.path;

                var plugin = pluginEngine.Get(id);
                if (plugin == null)
                {
                    return 404;
                }

                var data = rpcClient.Call<byte[]>(string.Format("{0}.resource.get", id), new[] {path});
                if (data == null)
                {
                    return 404;
                }

                var contentType = MimeTypes.GetMimeType(path);

                return Response.FromStream(() => new MemoryStream(data), contentType);
            };
        }
    }
}
