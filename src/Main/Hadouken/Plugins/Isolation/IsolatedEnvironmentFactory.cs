using System.IO;
using Hadouken.Configuration;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins.Isolation
{
    public class IsolatedEnvironmentFactory : IIsolatedEnvironmentFactory
    {
        public IIsolatedEnvironment CreateEnvironment(IConfiguration configuration, IManifest manifest)
        {
            var path = Path.Combine(configuration.Plugins.BaseDirectory,
                string.Concat(manifest.Name, "-", manifest.Version));

            return new AppDomainIsolatedEnvironment(path, manifest.AssemblyFile);
        }
    }
}