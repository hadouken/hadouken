using System;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Metadata;
using NLog;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IManifest _manifest;
        private readonly IIsolatedEnvironment _isolatedEnvironment;

        public PluginManager(IIsolatedEnvironment environment, IManifest manifest)
        {
            State = PluginState.Unloaded;

            _manifest = manifest;
            _isolatedEnvironment = environment;
        }

        public IManifest Manifest
        {
            get { return _manifest; }
        }

        public PluginState State { get; private set; }

        public string ErrorMessage { get; set; }

        public long GetMemoryUsage()
        {
            if (State != PluginState.Loaded)
                return -1;

            return _isolatedEnvironment.GetMemoryUsage();
        }

        public void Load()
        {
            Logger.Info("Loading plugin {0}", Manifest.Name);
            State = PluginState.Loading;

            try
            {
                _isolatedEnvironment.Load();
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
    }
}
