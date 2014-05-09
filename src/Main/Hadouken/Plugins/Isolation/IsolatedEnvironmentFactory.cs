using Hadouken.Fx.IO;

namespace Hadouken.Plugins.Isolation
{
    public class IsolatedEnvironmentFactory : IIsolatedEnvironmentFactory
    {
        public IIsolatedEnvironment CreateEnvironment(IDirectory directory)
        {
            return new ProcessIsolatedEnvironment(directory);
        }
    }
}