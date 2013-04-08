using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common;
using Hadouken.Common.Plugins;
using Hadouken.Common.Messaging;
using Hadouken.Common.DI;

namespace Hadouken.Plugins.PluginEngine
{
    public class PluginSandbox : MarshalByRefObject
    {
        private Plugin _plugin;

        public void AddAssemblies(IEnumerable<byte[]> assemblies)
        {
            foreach (var asm in assemblies)
            {
                AppDomain.CurrentDomain.Load(asm);
            }
        }

        public void Load(PluginManifest manifest)
        {
            var resolverType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                from type in asm.GetTypes()
                                where typeof (IDependencyResolver).IsAssignableFrom(type)
                                where type.IsClass && !type.IsAbstract
                                select type).First();

            var resolver = (IDependencyResolver) Activator.CreateInstance(resolverType);

            Kernel.SetResolver(resolver);
            Kernel.BindToFunc(() =>
                {
                    var factory = Kernel.Get<IMessageBusFactory>();
                    return factory.Create("hdkn.plugins." + manifest.Name.ToLowerInvariant());
                });

            _plugin = Kernel.Get<Plugin>();
            _plugin.Load();
        }

        public void Unload()
        {
            _plugin.Unload();
        }
    }
}
