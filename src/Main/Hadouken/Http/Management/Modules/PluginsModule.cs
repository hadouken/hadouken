using System.IO;
using System.Linq;
using Hadouken.Fx.JsonRpc;
using Hadouken.Http.Management.Models;
using Hadouken.Plugins;
using Nancy;
using System.Text;

namespace Hadouken.Http.Management.Modules
{
    public class PluginsModule : ModuleBase
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
                        MemoryUsage = plugin.GetMemoryUsage(),
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

            Get["/upload"] = _ => View["Upload"];

            Get["/{id}/{path*}"] = _ =>
            {
                string id = _.id;
                string path = _.path;
                bool hasExtension = Path.HasExtension(path);

                var plugin = pluginEngine.Get(id);
                if (plugin == null || plugin.State != PluginState.Loaded)
                {
                    return 404;
                }

                if (hasExtension)
                {
                    var data = rpcClient.Call<byte[]>(string.Format("{0}.resource.get", id), new[] { path });
                    if (data == null)
                    {
                        return 404;
                    }

                    var contentType = MimeTypes.GetMimeType(path);

                    return Response.FromStream(() => new MemoryStream(data), contentType);
                }

                // Return route instead
                if (!plugin.Manifest.UserInterface.Pages.ContainsKey(path))
                {
                    return 404;
                }

                var page = plugin.Manifest.UserInterface.Pages[path];
                var htmlData = rpcClient.Call<byte[]>(string.Format("{0}.resource.get", id), new[] {page.HtmlFile});

                if (htmlData == null)
                {
                    return 404;
                }

                return View["Template", new
                {
                    PluginId = plugin.Manifest.Name,
                    page.Scripts,
                    Template = Encoding.UTF8.GetString(htmlData)
                }];
            };
        }
    }
}
