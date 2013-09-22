using System;

using Hadouken.Framework;
using Hadouken.Sandbox;
using Hadouken.IO;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _path;
        private readonly IFileSystem _fileSystem;
        private IBootConfig _bootConfig;
        private SandboxedEnvironment _sandboxedEnvironment;

        public PluginManager(string path, IFileSystem fileSystem)
        {
            State = PluginState.Unknown;

            _path = path;
            _fileSystem = fileSystem;
            LoadManifest();
        }

        private void LoadManifest()
        {
            var manifestPath = Path.Combine(_path, "manifest.json");

            if (!_fileSystem.FileExists(manifestPath))
                throw new PluginManifestNotFoundException();

            Logger.Info("Loading manifest from {0}", manifestPath);

            using(var stream = _fileSystem.OpenRead(manifestPath))
            using(var reader = new StreamReader(stream))
            {
                try
                {
                    var manifest = JObject.Parse(reader.ReadToEnd());

                    Name = manifest["name"].Value<string>();
                    Version = new Version(manifest["version"].Value<string>());
                }
                catch (Exception e)
                {
                    Logger.ErrorException(String.Format("Could not parse manifest file {0}", manifestPath), e);
                    throw new PluginManifestParseException(e);
                }
            }
        }

        public string Name { get; private set; }

        public Version Version { get; private set; }

        public string[] DependsOn { get; private set; }

        public PluginState State { get; private set; }

        public void SetBootConfig(IBootConfig bootConfig)
        {
            _bootConfig = bootConfig;
        }

        public void Load()
        {
            Logger.Info("Loading plugin {0}", Name);

            State = PluginState.Loading;

            var setupInfo = new AppDomainSetup
                {
                    ApplicationBase = _path
                };

            var assemblyName = typeof (SandboxedEnvironment).Assembly.Location;
            var typeName = typeof (SandboxedEnvironment).FullName;

            var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setupInfo);

            Logger.Debug("Creating sandboxed environment");
            _sandboxedEnvironment = (SandboxedEnvironment) domain.CreateInstanceFromAndUnwrap(assemblyName, typeName);
            Logger.Debug("Loading {0} in sandboxed environment", Name);
            _sandboxedEnvironment.Load(_bootConfig);

            State = PluginState.Loaded;
        }

        public void Unload()
        {
            State = PluginState.Unloading;

            if (_sandboxedEnvironment == null) return;

            var domain = _sandboxedEnvironment.GetAppDomain();

            if (domain == null) return;

            Logger.Debug("Unloading AppDomain for plugin {0}", Name);
            AppDomain.Unload(domain);

            State = PluginState.Unloaded;
        }
    }
}
