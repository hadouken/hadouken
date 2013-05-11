using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hadouken.Reflection;
using Ninject.Modules;

namespace Hadouken.DI.Ninject.Modules
{
    public class ComponentModule : NinjectModule
    {
        private readonly IEnumerable<Assembly> _assemblies;
 
        public ComponentModule() : this(AppDomain.CurrentDomain.GetAssemblies())
        {
        }

        public ComponentModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public override void Load()
        {
            var componentTypes = (from asm in _assemblies
                                  from type in asm.GetTypes()
                                  where type.HasAttribute<ComponentAttribute>()
                                  where type.IsClass && !type.IsAbstract
                                  select type);

            foreach (var componentType in componentTypes)
            {
                var attr = componentType.GetAttribute<ComponentAttribute>();
                var style = (attr == null ? ComponentLifestyle.Singleton : attr.Lifestyle);
                var binding = Kernel.Bind(componentType.GetInterfaces()).To(componentType);

                switch (style)
                {
                    case ComponentLifestyle.Singleton:
                        binding.InTransientScope();
                        break;

                    case ComponentLifestyle.Transient:
                        binding.InSingletonScope();
                        break;
                }
            }
        }
    }
}
