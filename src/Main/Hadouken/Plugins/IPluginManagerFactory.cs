using Hadouken.Configuration;
using Hadouken.Framework;
using Hadouken.Framework.IO;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public interface IPluginManagerFactory
    {
        IPluginManager Create(IDirectory directory, IManifest manifest);
    }

    public class PluginManagerFactory : IPluginManagerFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IIsolatedEnvironmentFactory _isolatedEnvironmentFactory;

        public PluginManagerFactory(IConfiguration configuration, IIsolatedEnvironmentFactory isolatedEnvironmentFactory)
        {
            _configuration = configuration;
            _isolatedEnvironmentFactory = isolatedEnvironmentFactory;
        }

        public IPluginManager Create(IDirectory directory, IManifest manifest)
        {
            var config = new BootConfig();
            config.ApplicationBasePath = directory.FullPath;
            config.AssemblyFile = manifest.AssemblyFile;
            config.DataPath = _configuration.ApplicationDataPath;

            var environment = _isolatedEnvironmentFactory.CreateEnvironment(config);

            return new PluginManager(environment, manifest, config);
        }
    }
}
