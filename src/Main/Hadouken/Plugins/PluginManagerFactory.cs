using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Messaging;
using Hadouken.Plugins.Isolation;
using NuGet;
using ILogger = Serilog.ILogger;

namespace Hadouken.Plugins
{
    public class PluginManagerFactory : IPluginManagerFactory
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMessageQueue _messageQueue;
        private readonly IIsolatedEnvironmentFactory _isolatedEnvironmentFactory;

        public PluginManagerFactory(ILogger logger, IConfiguration configuration, IMessageQueue messageQueue, IIsolatedEnvironmentFactory isolatedEnvironmentFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _messageQueue = messageQueue;
            _isolatedEnvironmentFactory = isolatedEnvironmentFactory;
        }

        public IPluginManager Create(IDirectory directory, IPackage package)
        {
            var environment = _isolatedEnvironmentFactory.CreateEnvironment(directory);
            return new PluginManager(_logger.ForContext<PluginManager>(), _configuration, _messageQueue, directory, environment, package);
        }
    }
}