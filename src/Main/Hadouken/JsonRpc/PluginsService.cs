using System;
using System.IO;
using System.Linq;
using System.Text;
using Hadouken.Fx.JsonRpc;
using Hadouken.Plugins;

namespace Hadouken.JsonRpc
{
    public class PluginsService : IJsonRpcService
    {
        private readonly IPluginEngine _pluginEngine;
        private readonly IPackageReader _packageReader;

        public PluginsService(IPluginEngine pluginEngine, IPackageReader packageReader)
        {
            if (pluginEngine == null)
            {
                throw new ArgumentNullException("pluginEngine");
            }

            _pluginEngine = pluginEngine;
            _packageReader = packageReader;
        }

        [JsonRpcMethod("core.plugins.list")]
        public string[] ListPlugins()
        {
            return _pluginEngine.GetAll().Select(p => p.Manifest.Name).ToArray();
        }

        [JsonRpcMethod("core.plugins.getVersion")]
        public string GetVersion(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);
            if (plugin == null)
            {
                return null;
            }

            return plugin.Manifest.Version.ToString();
        }

        [JsonRpcMethod("core.plugins.install")]
        public bool Install(string base64EncodedPackage)
        {
            if (string.IsNullOrEmpty(base64EncodedPackage))
            {
                throw new ArgumentNullException("base64EncodedPackage");
            }

            var data = Convert.FromBase64String(base64EncodedPackage);

            using (var memoryStream = new MemoryStream(data))
            {
                var package = _packageReader.Read(memoryStream);
                return _pluginEngine.InstallOrUpgrade(package);
            }
        }

        [JsonRpcMethod("core.plugins.uninstall")]
        public bool Uninstall(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);

            if (plugin == null)
            {
                return false;
            }

            _pluginEngine.Unload(pluginId);
            _pluginEngine.Uninstall(pluginId);

            return true;
        }
    }
}
