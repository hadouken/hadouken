using Hadouken.Fx.IO;
using Serilog;

namespace Hadouken.Plugins.Isolation
{
    public class IsolatedEnvironmentFactory : IIsolatedEnvironmentFactory
    {
        private readonly ILogger _logger;

        public IsolatedEnvironmentFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IIsolatedEnvironment CreateEnvironment(IDirectory directory)
        {
            return new ProcessIsolatedEnvironment(_logger, directory);
        }
    }
}