using Hadouken.Fx.IO;

namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironmentFactory
    {
        IIsolatedEnvironment CreateEnvironment(IDirectory directory);
    }
}
