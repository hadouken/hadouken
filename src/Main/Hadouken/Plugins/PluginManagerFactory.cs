using System;
using System.Reflection;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Messaging;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;
using Hadouken.SemVer;

namespace Hadouken.Plugins
{
    public class PluginManagerFactory : IPluginManagerFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IMessageQueue _messageQueue;
        private readonly IIsolatedEnvironmentFactory _isolatedEnvironmentFactory;
        private readonly SemanticVersion _hostVersion;

        public PluginManagerFactory(IConfiguration configuration, IMessageQueue messageQueue, IIsolatedEnvironmentFactory isolatedEnvironmentFactory)
        {
            _configuration = configuration;
            _messageQueue = messageQueue;
            _isolatedEnvironmentFactory = isolatedEnvironmentFactory;

            var attr = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (attr != null)
            {
                _hostVersion = new SemanticVersion(attr.InformationalVersion);
            }
        }

        public IPluginManager Create(IDirectory directory, IManifest manifest)
        {
            var environment = _isolatedEnvironmentFactory.CreateEnvironment(directory);

            // Check minimum host version
            if (manifest.MinimumHostVersion > _hostVersion)
            {
                throw new PluginException("Plugin does not support host version: " + _hostVersion);
            }

            return new PluginManager(_configuration, _messageQueue, directory, environment, manifest);
        }
    }
}