using Hadouken.Framework;

namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironmentFactory
    {
        IIsolatedEnvironment CreateEnvironment(IPackage package, IBootConfig config);
    }
}
