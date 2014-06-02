using System;
using System.Collections.Generic;
using System.Linq;
using Hadouken.Configuration;
using Hadouken.Fx.JsonRpc;
using Hadouken.JsonRpc.Dto;
using Hadouken.Plugins;
using Hadouken.Security;

namespace Hadouken.JsonRpc
{
    public class PluginsService : IJsonRpcService
    {
        private readonly IConfiguration _configuration;
        private readonly IPluginEngine _pluginEngine;
        private readonly IAuthenticationManager _authenticationManager;

        public PluginsService(IConfiguration configuration, IPluginEngine pluginEngine, IAuthenticationManager authenticationManager)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (pluginEngine == null) throw new ArgumentNullException("pluginEngine");
            if (authenticationManager == null) throw new ArgumentNullException("authenticationManager");

            _configuration = configuration;
            _pluginEngine = pluginEngine;
            _authenticationManager = authenticationManager;
        }

        [JsonRpcMethod("core.plugins.list")]
        public IEnumerable<PluginListItem> ListPlugins()
        {
            var plugins = _pluginEngine.GetAll();

            return (from plugin in plugins
                select new PluginListItem
                {
                    Id = plugin.Package.Id,
                    Name = plugin.Package.Title ?? plugin.Package.Id,
                    State = plugin.State.ToString(),
                    Version = plugin.Package.Version.ToString()
                });
        }

        [JsonRpcMethod("core.plugins.getVersion")]
        public string GetVersion(string pluginId)
        {
            var plugin = _pluginEngine.Get(pluginId);
            if (plugin == null)
            {
                return null;
            }

            return plugin.Package.Version.ToString();
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
                plugin.Package.Id,
                Path = plugin.BaseDirectory.FullPath,
                State = plugin.State.ToString(),
                Version = plugin.Package.Version.ToString(),
                plugin.IsolatedEnvironment.Output,
                plugin.IsolatedEnvironment.ErrorOutput
            };
        }

        [JsonRpcMethod("core.plugins.install")]
        public void Install(string password, string packageId, string version, bool ignoreDependencies, bool allowPrereleaseVersions)
        {
            if (!_authenticationManager.IsValid(_configuration.UserName, password))
            {
                throw new JsonRpcException(9999, "Invalid username/password.");
            }

            _pluginEngine.Install(packageId, version, ignoreDependencies, allowPrereleaseVersions);
        }

        [JsonRpcMethod("core.plugins.uninstall")]
        public void Uninstall(string password, string packageId, string version, bool forceRemove, bool removeDependencies)
        {
            if (!_authenticationManager.IsValid(_configuration.UserName, password))
            {
                throw new JsonRpcException(9999, "Invalid username/password.");
            }

            _pluginEngine.Uninstall(packageId, version, forceRemove, removeDependencies);
        }

        [JsonRpcMethod("core.plugins.update")]
        public void Update(string password, string packageId, string version, bool updateDependencies, bool allowPrereleaseVersions)
        {
            if (!_authenticationManager.IsValid(_configuration.UserName, password))
            {
                throw new JsonRpcException(9999, "Invalid username/password.");
            }

            _pluginEngine.Update(packageId, version, updateDependencies, allowPrereleaseVersions);
        }
    }
}
