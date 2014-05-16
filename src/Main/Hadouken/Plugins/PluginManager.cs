using System;
using System.Collections.Generic;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Messaging;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Messages;
using Hadouken.Plugins.Metadata;
using Serilog;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private readonly IDictionary<string, object> _configuration;
        private readonly ILogger _logger;
        private readonly IMessageQueue _messageQueue;
        private readonly IDirectory _baseDirectory;
        private readonly IManifest _manifest;
        private readonly IIsolatedEnvironment _isolatedEnvironment;

        public PluginManager(ILogger logger, IConfiguration configuration, IMessageQueue messageQueue, IDirectory baseDirectory, IIsolatedEnvironment environment, IManifest manifest)
        {
            ErrorCount = 0;
            
            if (logger == null) throw new ArgumentNullException("logger");

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (baseDirectory == null)
            {
                throw new ArgumentNullException("baseDirectory");
            }

            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            if (manifest == null)
            {
                throw new ArgumentNullException("manifest");
            }

            State = PluginState.Unloaded;

            _logger = logger;
            _messageQueue = messageQueue;
            _baseDirectory = baseDirectory;
            _manifest = manifest;
            _isolatedEnvironment = environment;
            _configuration = BuildConfiguration(configuration);

            _isolatedEnvironment.UnhandledError += OnUnhandledError;
        }

        public IDirectory BaseDirectory
        {
            get { return _baseDirectory; }
        }

        public IManifest Manifest
        {
            get { return _manifest; }
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
            _logger.Information("Loading plugin {Name}", Manifest.Name);
            State = PluginState.Loading;

            try
            {
                _isolatedEnvironment.Load(_configuration);
                State = PluginState.Loaded;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when loading plugin {Name}", Manifest.Name);
                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }

        public void Unload()
        {
            _logger.Information("Unloading plugin {Name}", Manifest.Name);
            State = PluginState.Unloading;

            try
            {
                _isolatedEnvironment.Unload();
                State = PluginState.Unloaded;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when unloading plugin {Name}", Manifest.Name);
                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }

        private void OnUnhandledError(object sender, EventArgs eventArgs)
        {
            ErrorMessage = "Plugin crashed unexpectedly.";
            State = PluginState.Error;

            ErrorCount += 1;

            _messageQueue.Publish(new PluginErrorMessage(Manifest.Name));
        }

        private IDictionary<string, object> BuildConfiguration(IConfiguration configuration)
        {
            return new Dictionary<string, object>
            {
                { "Name", _manifest.Name },
                { "Url", "hadouken://plugins." + _manifest.Name },
                { "RpcTemplate", configuration.Rpc.PluginUriTemplate },
                { "Permissions", _manifest.Permissions.ToXml().ToString() }
            };
        }
    }
}
