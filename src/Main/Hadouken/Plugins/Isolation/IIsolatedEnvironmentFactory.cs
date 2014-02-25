using Hadouken.Configuration;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins.Isolation
{
    public interface IIsolatedEnvironmentFactory
    {
        IIsolatedEnvironment CreateEnvironment(IConfiguration configuration, IManifest manifest);
    }
}
