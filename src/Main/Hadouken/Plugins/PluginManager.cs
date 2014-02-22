using System;
using Hadouken.Framework;
using Hadouken.Plugins.Isolation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;
using Hadouken.Framework.Rpc;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings();

        private readonly IPackage _package;
        private readonly IBootConfig _bootConfig;
        private readonly IJsonRpcClient _rpcClient;
        private readonly IIsolatedEnvironment _isolatedEnvironment;

        static PluginManager()
        {
            SerializerSettings.Converters.Add(new VersionConverter());
        }

        public PluginManager(IIsolatedEnvironmentFactory environmentFactory, IPackage package, IBootConfig bootConfig, IJsonRpcClient rpcClient)
        {
            State = PluginState.Unloaded;

            _package = package;
            _bootConfig = bootConfig;
            _rpcClient = rpcClient;
            _isolatedEnvironment = environmentFactory.CreateEnvironment(package, bootConfig);
        }

        public IPackage Package
        {
            get { return _package; }
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
            Logger.Info("Loading plugin {0}", Package.Manifest.Name);

            State = PluginState.Loading;

            try
            {
                _isolatedEnvironment.Load();

                State = PluginState.Loaded;

                _rpcClient.Call<bool>("events.publish",
                    new object[]
                    {"plugin.loaded", new {name = Package.Manifest.Name, version = Package.Manifest.Version}});
            }
            catch (Exception e)
            {
                Logger.Error("Error when loading plugin " + Package.Manifest.Name, e);
                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }

        public void Unload()
        {
            State = PluginState.Unloading;

            try
            {
                _isolatedEnvironment.Unload();

                State = PluginState.Unloaded;

                _rpcClient.Call<bool>("events.publish", new object[] {"plugin.unloaded", Package.Manifest.Name});
            }
            catch (Exception e)
            {
                Logger.Error("Error when unloading plugin " + Package.Manifest.Name, e);

                ErrorMessage = e.Message;
                State = PluginState.Error;
            }
        }
    }
}
