using System;
using System.IO;
using System.Linq;
using Hadouken.Plugins;
using System.Reflection;

namespace Hadouken.Http.Api
{
    [ApiAction("getpluginfile")]
    public class GetPluginFile : ApiAction
    {
        private readonly IPluginEngine _pluginEngine;

        public GetPluginFile(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public override ActionResult Execute()
        {
            var plugin = Context.Request.QueryString["plugin"];
            var file = Context.Request.QueryString["file"];

            if (String.IsNullOrEmpty(plugin) || String.IsNullOrEmpty(file))
                return FileNotFound();

            if (!_pluginEngine.Managers.ContainsKey(plugin))
                return FileNotFound();

            var manager = _pluginEngine.Managers[plugin];
            var resource = manager.GetResource(file);

            if (resource != null && resource.Length > 0)
                return new ContentResult() {Content = resource, ContentType = GetContentType(Path.GetExtension(file))};

            return FileNotFound();
        }

        private string GetContentType(string extension)
        {
            switch(extension)
            {
                case ".css":
                    return "text/css";

                case ".js":
                    return "text/javascript";

                case ".png":
                    return "image/png";

                case ".gif":
                    return "image/gif";

                case ".html":
                    return "text/html";
            }

            return "text/html";
        }
    }
}
