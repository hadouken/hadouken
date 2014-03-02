using System.IO;
using System.Linq;
using Hadouken.Http;

namespace Hadouken.Plugins.Repository
{
    public class PackageDownloader : IPackageDownloader
    {
        private readonly IPluginRepository _pluginRepository;
        private readonly IApiConnection _apiConnection;
        private readonly IPackageReader _packageReader;

        public PackageDownloader(IPluginRepository pluginRepository, IApiConnection apiConnection, IPackageReader packageReader)
        {
            _pluginRepository = pluginRepository;
            _apiConnection = apiConnection;
            _packageReader = packageReader;
        }

        public IPackage Download(string packageId)
        {
            var plugin = _pluginRepository.GetById(packageId);
            if (plugin == null)
            {
                return null;
            }

            var latestRelease = plugin.Releases.First();
            if (latestRelease == null)
            {
                return null;
            }

            var data = _apiConnection.DownloadData(latestRelease.DownloadUri);
            if (data == null)
            {
                return null;
            }

            using (var ms = new MemoryStream(data))
            {
                return _packageReader.Read(ms);
            }
        }
    }
}