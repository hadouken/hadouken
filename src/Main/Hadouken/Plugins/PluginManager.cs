using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Messaging;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Messages;
using Hadouken.Plugins.Metadata;
using NuGet;
using ILogger = Serilog.ILogger;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IMessageQueue _messageQueue;
        private readonly IDirectory _baseDirectory;
        private readonly IIsolatedEnvironment _isolatedEnvironment;
        private readonly IPackage _package;
        private readonly Lazy<IManifest> _lazyManifest; 

        public PluginManager(ILogger logger, IConfiguration configuration, IMessageQueue messageQueue, IDirectory baseDirectory, IIsolatedEnvironment environment, IPackage package)
        {
            ErrorCount = 0;
            
            if (logger == null) throw new ArgumentNullException("logger");
            if (configuration == null) throw new ArgumentNullException("configuration");
            if (baseDirectory == null) throw new ArgumentNullException("baseDirectory");
            if (environment == null) throw new ArgumentNullException("environment");
            if (package == null) throw new ArgumentNullException("package");

            State = PluginState.Unloaded;

            _logger = logger;
            _configuration = configuration;
            _messageQueue = messageQueue;
            _baseDirectory = baseDirectory;
            _isolatedEnvironment = environment;
            _package = package;
            _lazyManifest = new Lazy<IManifest>(_package.GetManifest);

            _isolatedEnvironment.UnhandledError += OnUnhandledError;
        }

        public IDirectory BaseDirectory
        {
            get { return _baseDirectory; }
        }

        public IPackage Package
        {
            get { return _package; }
        }

        public IManifest Manifest
        {
            get { return _lazyManifest.Value; }
        }

        public PluginState State { get; private set; }

        public int ErrorCount { get; private set; }

        public string ErrorMessage { get; set; }

        public long GetMemoryUsage()
        {
            if (State == PluginState.Loaded)
                return _isolatedEnvironment.GetMemoryUsage();

            return -1;
        }

        public void Load()
        {
            _logger.Information("Loading plugin {Name}", _package.Id);
            State = PluginState.Loading;

            try
            {
                var configuration = BuildConfiguration(_configuration);
                _isolatedEnvironment.Load(configuration);
                State = PluginState.Loaded;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when loading plugin {Name}", _package.Id);
                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }

        public void Unload()
        {
            _logger.Information("Unloading plugin {Name}", _package.Id);
            State = PluginState.Unloading;

            try
            {
                _isolatedEnvironment.Unload();
                State = PluginState.Unloaded;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when unloading plugin {Name}", _package.Id);
                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }

        private void OnUnhandledError(object sender, EventArgs eventArgs)
        {
            ErrorMessage = "Plugin crashed unexpectedly.";
            State = PluginState.Error;

            ErrorCount += 1;

            _messageQueue.Publish(new PluginErrorMessage(Package.Id));
        }

        private IDictionary<string, object> BuildConfiguration(IConfiguration configuration)
        {
            return new Dictionary<string, object>
            {
                { "Name", Package.Id },
                { "Url", "hadouken://plugins." + Package.Id },
                { "RpcTemplate", configuration.PluginUrlTemplate },
                { "Permissions", (Manifest != null && Manifest.Permissions != null) ? Manifest.Permissions.ToXml().ToString() : null }
            };
        }
    }
}
