using System;
using System.IO;
using Hadouken.Configuration;
using Hadouken.Fx;
using Hadouken.Fx.IO;
using Hadouken.Messaging;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Messages;
using Hadouken.Plugins.Metadata;
using NLog;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly PluginConfiguration _configuration;
        private readonly IMessageQueue _messageQueue;
        private readonly IDirectory _baseDirectory;
        private readonly IManifest _manifest;
        private readonly IIsolatedEnvironment _isolatedEnvironment;

        public PluginManager(IConfiguration configuration, IMessageQueue messageQueue, IDirectory baseDirectory, IIsolatedEnvironment environment, IManifest manifest)
        {
            ErrorCount = 0;
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
            Logger.Info("Loading plugin {0}", Manifest.Name);
            State = PluginState.Loading;

            try
            {
                _isolatedEnvironment.Load(_configuration);
                State = PluginState.Loaded;
            }
            catch (Exception e)
            {
                Logger.Error("Error when loading plugin " + Manifest.Name, e);
                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }

        public void Unload()
        {
            Logger.Info("Unloading plugin {0}", Manifest.Name);
            State = PluginState.Unloading;

            try
            {
                _isolatedEnvironment.Unload();
                State = PluginState.Unloaded;
            }
            catch (Exception e)
            {
                Logger.Error("Error when unloading plugin " + Manifest.Name, e);
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

        private PluginConfiguration BuildConfiguration(IConfiguration configuration)
        {
            return new PluginConfiguration
            {
                DataPath = Path.Combine(configuration.ApplicationDataPath, _manifest.Name),
                HttpHostBinding = configuration.Http.HostBinding,
                HttpPort = configuration.Http.Port,
                PluginName = _manifest.Name
            };
        }
    }
}
