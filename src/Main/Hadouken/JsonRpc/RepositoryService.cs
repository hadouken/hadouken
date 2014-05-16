using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Fx.JsonRpc;
using Hadouken.Http.Api;
using Hadouken.Http.Api.Models;
using Hadouken.Plugins;

namespace Hadouken.JsonRpc
{
    public sealed class RepositoryService : IJsonRpcService
    {
        private readonly IPluginRepository _pluginRepository;
        private readonly IPackageDownloader _packageDownloader;
        private readonly IPluginEngine _pluginEngine;

        public RepositoryService(IPluginRepository pluginRepository, 
            IPackageDownloader packageDownloader,
            IPluginEngine pluginEngine)
        {
            _pluginRepository = pluginRepository;
            _packageDownloader = packageDownloader;
            _pluginEngine = pluginEngine;
        }

        [JsonRpcMethod("core.repository.list")]
        public IEnumerable<PluginListItem> List()
        {
            return _pluginRepository.GetAll();
        }

        [JsonRpcMethod("core.repository.details")]
        public Plugin Details(string pluginId)
        {
            return _pluginRepository.GetById(pluginId);
        }

        [JsonRpcMethod("core.repository.install")]
        public object Install(string pluginId)
        {
            var package = _packageDownloader.Download(pluginId);

            if (package == null)
            {
                return new { result = false, message = "Error when downloading package." };
            }

            var installResult = _pluginEngine.InstallOrUpgrade(package);

            if (!installResult)
            {
                return new { result = false, message = "Failed to install package." };
            }

            return new { result = true };
        }
    }
}
