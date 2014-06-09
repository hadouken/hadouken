using System;
using System.Linq;
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

        public RepositoryService(IPackageRepository packageRepository, IPackageManager packageManager)
        {
            if (packageRepository == null) throw new ArgumentNullException("packageRepository");
            if (packageManager == null) throw new ArgumentNullException("packageManager");
            _packageRepository = packageRepository;
            _packageManager = packageManager;
        }

        [JsonRpcMethod("core.repository.search")]
        public RepositorySearchResult Search(int page, string query, bool allowPrerelease)
        {
            var count = _packageRepository.Search(query, allowPrerelease).Count();
            var pages = (count + DefaultPageSize - 1)/DefaultPageSize;

            var packages = _packageRepository.Search(query, allowPrerelease)
                .Skip((page - 1)*DefaultPageSize)
                .Take(DefaultPageSize)
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
                    IsInstalled = _packageManager.LocalRepository.FindPackage(p.Id, p.Version) != null
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
                Dependencies = package.DependencySets.SelectMany(dep => dep.Dependencies).Select(dep => new {dep.Id, Version = VersionUtility.PrettyPrint(dep.VersionSpec)}),
                Manifest = package.GetManifest(),
                package.Description,
                Title = package.Title ?? package.Id,
                Version = package.Version.ToString(),
            };
        }
    }
}
