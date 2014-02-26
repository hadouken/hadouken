using Hadouken.Fx.IO;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins
{
    public class PluginManagerFactory : IPluginManagerFactory
    {
        private readonly IIsolatedEnvironmentFactory _isolatedEnvironmentFactory;

        public PluginManagerFactory(IIsolatedEnvironmentFactory isolatedEnvironmentFactory)
        {
            _isolatedEnvironmentFactory = isolatedEnvironmentFactory;
        }

        public IPluginManager Create(IDirectory directory, IManifest manifest)
        {
            var environment = _isolatedEnvironmentFactory.CreateEnvironment(directory, manifest);
            return new PluginManager(directory, environment, manifest);
        }
    }
}