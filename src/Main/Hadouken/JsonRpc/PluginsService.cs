using System;
using System.IO;
using System.Linq;
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
            if (pluginEngine == null) throw new ArgumentNullException("pluginEngine");
            if (packageReader == null) throw new ArgumentNullException("packageReader");

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

        [JsonRpcMethod("core.plugins.load")]
        public bool Load(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);

            if (plugin == null)
            {
                return false;
            }

            _pluginEngine.Load(pluginId);

            return true;
        }

        [JsonRpcMethod("core.plugins.unload")]
        public bool Unload(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);

            if (plugin == null)
            {
                return false;
            }

            _pluginEngine.Unload(pluginId);

            return true;
        }

        [JsonRpcMethod("core.plugins.getDetails")]
        public object GetDetails(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);

            if (plugin == null)
            {
                return null;
            }

            return new
            {
                plugin.Manifest.Name,
                Path = plugin.BaseDirectory.FullPath,
                State = plugin.State.ToString(),
                Version = plugin.Manifest.Version.ToString()
            };
        }
    }
}
