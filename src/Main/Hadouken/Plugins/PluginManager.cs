using System;
using System.Collections.Generic;
using System.IO;
using Hadouken.Framework;
using Hadouken.Plugins.Metadata;
using Hadouken.Sandbox;
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
        private SandboxedEnvironment _sandboxedEnvironment;
        private readonly IJsonRpcClient _rpcClient;

        static PluginManager()
        {
            SerializerSettings.Converters.Add(new VersionConverter());
        }

        public PluginManager(IPackage package, IBootConfig bootConfig, IJsonRpcClient rpcClient)
        {
            State = PluginState.Unloaded;

            _package = package;
            _bootConfig = bootConfig;
            _rpcClient = rpcClient;
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

            var domain = _sandboxedEnvironment.GetAppDomain();

            if (domain == null)
                return -1;

            return domain.MonitoringSurvivedMemorySize;
        }

        public async void Load()
        {
            Logger.Info("Loading plugin {0}", Package.Manifest.Name);

            State = PluginState.Loading;

            var setupInfo = new AppDomainSetup
                {
                };

            var assemblyName = typeof (SandboxedEnvironment).Assembly.Location;
            var typeName = typeof (SandboxedEnvironment).FullName;
            var domainName = String.Concat(Package.Manifest.Name, "-", Package.Manifest.Version);
            var domain = AppDomain.CreateDomain(domainName, null, setupInfo);

            Logger.Debug("Creating sandboxed environment");
            _sandboxedEnvironment = (SandboxedEnvironment) domain.CreateInstanceFromAndUnwrap(assemblyName, typeName);

            Logger.Debug("Loading {0} in sandboxed environment", Package.Manifest.Name);

            var assemblies = new List<byte[]>();

            foreach (var file in _package.Files)
            {
                if (file.Extension != ".dll")
                    continue;

                using (var stream = file.OpenRead())
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    assemblies.Add(ms.ToArray());
                }
            }

            _sandboxedEnvironment.Load(_bootConfig, assemblies.ToArray());

            State = PluginState.Loaded;

            await _rpcClient.CallAsync<bool>("events.publish", new object[] { "plugin.loaded", new { name = Package.Manifest.Name, version = Package.Manifest.Version } });
        }

        public void Unload()
        {
            State = PluginState.Unloading;

            if (_sandboxedEnvironment == null) return;

            var domain = _sandboxedEnvironment.GetAppDomain();

            if (domain == null) return;

            Logger.Debug("Unloading AppDomain for plugin {0}", Package.Manifest.Name);
            AppDomain.Unload(domain);

            State = PluginState.Unloaded;

            _rpcClient.CallAsync<bool>("events.publish", new object[] { "plugin.unloaded", Package.Manifest.Name }).Wait();
        }
    }
}
