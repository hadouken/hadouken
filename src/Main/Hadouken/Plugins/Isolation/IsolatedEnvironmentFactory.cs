using Hadouken.Framework;

namespace Hadouken.Plugins.Isolation
{
    public class IsolatedEnvironmentFactory : IIsolatedEnvironmentFactory
    {
        public IIsolatedEnvironment CreateEnvironment(IPackage package, IBootConfig config)
        {
            return new AppDomainIsolatedEnvironment(package, config);
        }
    }
}