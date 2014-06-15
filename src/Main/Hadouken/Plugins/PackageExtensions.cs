using System.Linq;
using Hadouken.Plugins.Metadata;
using NuGet;
using Manifest = Hadouken.Plugins.Metadata.Manifest;

namespace Hadouken.Plugins
{
    public static class PackageExtensions
    {
        public static IManifest GetManifest(this IPackage package)
        {
            var file = package.GetFiles().SingleOrDefault(f => f.Path == "manifest.json");
            if (file == null) return null;

            IManifest manifest;
            return !Manifest.TryParse(file.GetStream(), out manifest) ? null : manifest;
        }
    }
}
