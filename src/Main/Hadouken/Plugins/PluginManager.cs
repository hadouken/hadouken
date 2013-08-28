using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Sandbox;
using System.Security;

namespace Hadouken.Plugins
{
    public sealed class PluginManager : IPluginManager
    {
        private readonly string _path;
        private SandboxedEnvironment _sandboxedEnvironment;

        public PluginManager(string path)
        {
            _path = path;
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public Version Version
        {
            get { throw new NotImplementedException(); }
        }

        public PluginState State
        {
            get { throw new NotImplementedException(); }
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
            _sandboxedEnvironment.Load();
        }

        public void Unload()
        {
            if (_sandboxedEnvironment == null) return;

            var domain = _sandboxedEnvironment.GetAppDomain();
            AppDomain.Unload(domain);
        }
    }
}
