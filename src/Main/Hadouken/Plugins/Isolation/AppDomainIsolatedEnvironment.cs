using System;
using System.Collections.Generic;
using System.IO;
using Hadouken.Framework;
using Hadouken.Sandbox;
using NLog;

namespace Hadouken.Plugins.Isolation
{
    public class AppDomainIsolatedEnvironment : IIsolatedEnvironment
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IPackage _package;
        private readonly IBootConfig _config;
        private SandboxedEnvironment _sandbox;

        public AppDomainIsolatedEnvironment(IPackage package, IBootConfig config)
        {
            _package = package;
            _config = config;
        }

        public void Load()
        {
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

            var setupInfo = new AppDomainSetup
            {
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
            };

            var assemblyName = typeof(SandboxedEnvironment).Assembly.Location;
            var typeName = typeof(SandboxedEnvironment).FullName;
            var domainName = String.Concat(_package.Manifest.Name, "-", _package.Manifest.Version);
            var domain = AppDomain.CreateDomain(domainName, null, setupInfo);

            Logger.Debug("Creating sandboxed environment");
            _sandbox = (SandboxedEnvironment)domain.CreateInstanceFromAndUnwrap(assemblyName, typeName);

            Logger.Debug("Loading {0} in sandboxed environment", _package.Manifest.Name);

            _sandbox.Load(_config, assemblies.ToArray());
        }

        public void Unload()
        {
            if (_sandbox == null) return;

            var domain = _sandbox.GetAppDomain();

            if (domain == null) return;

            Logger.Debug("Unloading AppDomain for plugin {0}", _package.Manifest.Name);
            AppDomain.Unload(domain);
        }

        public long GetMemoryUsage()
        {
            var domain = _sandbox.GetAppDomain();

            if (domain == null)
                return -1;

            return domain.MonitoringTotalAllocatedMemorySize;
        }
    }
}