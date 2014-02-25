using Hadouken.Configuration;
using Hadouken.Fx.IO;
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
            var environment = _isolatedEnvironmentFactory.CreateEnvironment(directory, manifest);
            return new PluginManager(environment, manifest);
        }
    }
}
