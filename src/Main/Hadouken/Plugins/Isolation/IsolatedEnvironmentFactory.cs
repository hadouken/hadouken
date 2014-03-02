using System.IO;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Plugins.Metadata;

namespace Hadouken.Plugins.Isolation
{
    public class IsolatedEnvironmentFactory : IIsolatedEnvironmentFactory
    {
        public IIsolatedEnvironment CreateEnvironment(IDirectory directory, IManifest manifest)
        {
            return new AppDomainIsolatedEnvironment(directory.FullPath);
        }
    }
}