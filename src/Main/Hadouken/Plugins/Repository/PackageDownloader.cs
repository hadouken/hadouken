using System;
using System.IO;
using Hadouken.Configuration;
using Hadouken.Http;
using Newtonsoft.Json;

namespace Hadouken.Plugins.Repository
{
    public class PackageDownloader : IPackageDownloader
    {
        public class PluginDto
        {
            public string Name { get; set; }

            [JsonProperty("latest_release")]
            public PluginLatestReleaseDto LatestRelease { get; set; }
        }

        public class PluginLatestReleaseDto
        {
            [JsonProperty("download_uri")]
            public Uri DownloadUri { get; set; }
        }

        private readonly IConfiguration _configuration;
        private readonly IApiConnection _apiConnection;

        public PackageDownloader(IConfiguration configuration, IApiConnection apiConnection)
        {
            _configuration = configuration;
            _apiConnection = apiConnection;
        }

        public IPackage Download(string packageId)
        {
            var pluginUri = new Uri(_configuration.Plugins.RepositoryUri, "plugins/" + packageId);
            var dto = _apiConnection.GetAsync<PluginDto>(pluginUri).Result;

            if (dto == null || dto.LatestRelease == null)
                return null;

            var data = _apiConnection.DownloadData(dto.LatestRelease.DownloadUri);

            if (data == null)
                return null;

            using (var ms = new MemoryStream(data))
            {
                IPackage package;
                return !Package.TryParse(ms, out package) ? null : package;
            }
        }
    }
}