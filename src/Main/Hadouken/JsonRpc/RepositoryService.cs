using System;
using System.IO;
using System.Linq;
using System.Text;
using Hadouken.Fx.JsonRpc;
using Hadouken.JsonRpc.Dto;
using Hadouken.Plugins;
using NuGet;

namespace Hadouken.JsonRpc
{
    public class RepositoryService : IJsonRpcService
    {
        private static readonly int DefaultPageSize = 10;
        private static readonly int DefaultMaxPageCount = 10;

        private readonly IPackageRepository _packageRepository;
        private readonly IPackageManager _packageManager;
        private readonly IJsonSerializer _jsonSerializer;

        public RepositoryService(IPackageRepository packageRepository, IPackageManager packageManager, IJsonSerializer jsonSerializer)
        {
            if (packageRepository == null) throw new ArgumentNullException("packageRepository");
            if (packageManager == null) throw new ArgumentNullException("packageManager");
            _packageRepository = packageRepository;
            _packageManager = packageManager;
            _jsonSerializer = jsonSerializer;
        }

        [JsonRpcMethod("core.repository.search")]
        public RepositorySearchResult Search(int page, string searchTerm, bool allowPrerelease)
        {
            var count = _packageRepository.Search(searchTerm, allowPrerelease).Count();
            var pages = (count + DefaultPageSize - 1)/DefaultPageSize;

            var query = _packageRepository.Search(searchTerm, allowPrerelease)
                .Where(p => p.IsAbsoluteLatestVersion)
                .AsCollapsed();

           var packages = query.OrderByDescending(p => p.DownloadCount)
                .ToList();

            return new RepositorySearchResult
            {
                TotalPages = Math.Min(pages, DefaultMaxPageCount),
                Items = packages.Select(p => new PackageListItem
                {
                    Id = p.Id,
                    Title = p.Title ?? p.Id,
                    Authors = string.Join(", ", p.Authors),
                    IconUrl = p.IconUrl,
                    Summary = p.Summary ?? p.Description,
                    Version = p.Version.ToString(),
                    IsInstalled = _packageManager.LocalRepository.Exists(p.Id, p.Version)
                })
            };
        }

        [JsonRpcMethod("core.repository.getDetails")]
        public object GetDetails(string packageId, string version)
        {
            var semver = SemanticVersion.Parse(version);
            var package = _packageRepository.FindPackage(packageId, semver);

            if (package == null)
            {
                return null;
            }

            return new
            {
                package.Id,
                Dependencies = package.DependencySets.SelectMany(dep => dep.Dependencies).Select(dep => new {dep.Id, Version = VersionUtility.PrettyPrint(dep.VersionSpec)}).ToArray(),
                Permissions = package.GetManifest().Permissions.ToString(),
                package.Description,
                Title = package.Title ?? package.Id,
                Version = package.Version.ToString(),
            };
        }
    }
}
