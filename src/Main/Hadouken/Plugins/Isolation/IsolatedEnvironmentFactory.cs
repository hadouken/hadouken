using Hadouken.Framework;

namespace Hadouken.Plugins.Isolation
{
    public class IsolatedEnvironmentFactory : IIsolatedEnvironmentFactory
    {
        public IIsolatedEnvironment CreateEnvironment(IBootConfig config)
        {
            return new AppDomainIsolatedEnvironment(config);
        }
    }
}