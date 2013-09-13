using System;

using Hadouken.Framework;
using Hadouken.Sandbox;
using Hadouken.IO;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
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
            using(var stream = _fileSystem.OpenRead(Path.Combine(_path, "manifest.json")))
            using(var reader = new StreamReader(stream))
            {
                var manifest = JObject.Parse(reader.ReadToEnd());

                Name = manifest["name"].Value<string>();
                Version = new Version(manifest["version"].Value<string>());
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
            State = PluginState.Loading;

            var setupInfo = new AppDomainSetup
                {
                    ApplicationBase = _path
                };

            var assemblyName = typeof (SandboxedEnvironment).Assembly.Location;
            var typeName = typeof (SandboxedEnvironment).FullName;

            var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setupInfo);

            _sandboxedEnvironment = (SandboxedEnvironment) domain.CreateInstanceFromAndUnwrap(assemblyName, typeName);
            _sandboxedEnvironment.Load(_bootConfig);

            State = PluginState.Loaded;
        }

        public void Unload()
        {
            State = PluginState.Unloading;

            if (_sandboxedEnvironment == null) return;

            var domain = _sandboxedEnvironment.GetAppDomain();
            AppDomain.Unload(domain);

            State = PluginState.Unloaded;
        }
    }
}
