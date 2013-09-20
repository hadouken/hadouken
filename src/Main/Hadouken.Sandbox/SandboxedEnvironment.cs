using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;

namespace Hadouken.Sandbox
{
    public sealed class SandboxedEnvironment : MarshalByRefObject
    {
        private Plugin _plugin;

        public SandboxedEnvironment()
        {
            AppDomain.CurrentDomain.DomainUnload += (s, e) => Unload();
        }

        public override object InitializeLifetimeService()
        {
            // We do not want this to be garbage collected.
            return null;
        }

        public AppDomain GetAppDomain()
        {
            return AppDomain.CurrentDomain;
        }

        public void Load(IBootConfig bootConfig)
        {
            var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                                      .Select(File.ReadAllBytes)
                                      .Select(Assembly.Load).ToList();

            // Find the bootstrapper class in this AppDomain. Get the plugin and load it.
            var bootstrapperType = (from asm in assemblies
                                    from type in asm.GetTypes()
                                    where typeof(Bootstrapper).IsAssignableFrom(type)
                                    where type.IsClass && !type.IsAbstract
                                    select type).Single();

            var bootstrapper = (Bootstrapper)Activator.CreateInstance(bootstrapperType);

            _plugin = bootstrapper.Load(bootConfig);
            _plugin.Load();
        }

        private void Unload()
        {
            _plugin.Unload();
        }
    }
}
