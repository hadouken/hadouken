using System.IO;
using Hadouken.Fx.JsonRpc;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public sealed class PluginsModule : NancyModule
    {
        public PluginsModule(IPluginEngine pluginEngine, IJsonRpcClient rpcClient)
            : base("plugins")
        {
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
