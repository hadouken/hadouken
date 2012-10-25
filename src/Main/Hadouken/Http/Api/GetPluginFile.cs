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

            if (HasResource(manager.PluginType.Assembly, manager.ResourceBase + "." + file))
                return GetResource(manager.PluginType.Assembly, manager.ResourceBase + "." + file);

            return FileNotFound();
        }

        private ActionResult GetResource(Assembly assembly, string resourceName)
        {
            using(var ms = new MemoryStream())
            using(var resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream != null)
                {
                    resourceStream.CopyTo(ms);
                    var data = ms.ToArray();

                    return new ContentResult() { Content = data, ContentType = GetContentType(Path.GetExtension(resourceName)) };
                }
            }

            return null;
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

        private bool HasResource(Assembly assembly, string resourceName)
        {
            return assembly.GetManifestResourceNames().Any(mrn => mrn == resourceName);
        }
    }
}
