using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject.Modules;
using Hadouken.Plugins;

namespace Hadouken.DI.Ninject.Modules
{
    public class PluginModule : NinjectModule
    {
        private readonly IEnumerable<Assembly> _assemblies;
 
        public PluginModule() : this(AppDomain.CurrentDomain.GetAssemblies())
        {
        }

        public PluginModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public override void Load()
        {
            var pluginType = (from asm in _assemblies
                              from type in asm.GetTypes()
                              where type.IsClass && !type.IsAbstract
                              where typeof (IPlugin).IsAssignableFrom(type)
                              select type).FirstOrDefault();

            if (pluginType == null)
                return;

            Kernel.Bind<IPlugin>().To(pluginType);
        }
    }
}
