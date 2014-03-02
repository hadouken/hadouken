using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironmentFactory
    {
        IIsolatedEnvironment CreateEnvironment(IDirectory directory, IManifest manifest);
    }
}
