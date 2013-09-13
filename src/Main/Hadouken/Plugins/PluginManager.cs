using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework;
using Hadouken.Sandbox;
using System.Security;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private readonly string _path;
        private IBootConfig _bootConfig;
        private SandboxedEnvironment _sandboxedEnvironment;

        public PluginManager(string path)
        {
            _path = path;
        }

        public string Name
        {
            get { return ""; }
        }

        public Version Version
        {
            get { return new Version("1.0"); }
        }

        public PluginState State
        {
            get { throw new NotImplementedException(); }
        }

        public void SetBootConfig(IBootConfig bootConfig)
        {
            _bootConfig = bootConfig;
        }

        public void Load()
        {
            var setupInfo = new AppDomainSetup
                {
                    ApplicationBase = _path
                };

            var assemblyName = typeof (SandboxedEnvironment).Assembly.Location;
            var typeName = typeof (SandboxedEnvironment).FullName;

            var domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, setupInfo);

            _sandboxedEnvironment = (SandboxedEnvironment) domain.CreateInstanceFromAndUnwrap(assemblyName, typeName);
            _sandboxedEnvironment.Load(_bootConfig);
        }

        public void Unload()
        {
            if (_sandboxedEnvironment == null) return;

            var domain = _sandboxedEnvironment.GetAppDomain();
            AppDomain.Unload(domain);
        }
    }
}
