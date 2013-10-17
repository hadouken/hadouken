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
        private IPluginHost _pluginHost;

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
            // If we find a bootstrapper which is not the DefaultBootstrapper, use that instead.

            var bootstrapperTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from type in asm.GetTypes()
                where typeof (Bootstrapper).IsAssignableFrom(type)
                where type.IsClass && !type.IsAbstract
                select type).ToList();

            var bootstrapperType = bootstrapperTypes.Single(t => t == typeof (DefaultBootstrapper));

            if (bootstrapperTypes.Count > 1)
            {
                bootstrapperType = bootstrapperTypes.Single(t => t != typeof (DefaultBootstrapper));
            }

            var bootstrapper = (Bootstrapper)Activator.CreateInstance(bootstrapperType);

            _pluginHost = bootstrapper.Bootstrap(bootConfig);
            _pluginHost.Load();
        }

        private void Unload()
        {
            _pluginHost.Unload();
        }
    }
}
