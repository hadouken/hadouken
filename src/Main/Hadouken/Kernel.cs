using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.DI;
using Hadouken.Reflection;
using System.Reflection;

namespace Hadouken
{
    public static class Kernel
    {
        private static IDependencyResolver _resolver;

        public static void Register(params Assembly[] assemblies)
        {
            // get all interface types which are assignable from IComponent (but not IComponent itself)

            var componentTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                  from type in asm.GetTypes()
                                  where typeof(IComponent).IsAssignableFrom(type)
                                  where type != typeof(IComponent) && type.IsInterface
                                  select type);

            foreach (var component in componentTypes)
            {
                var lifestyle = ComponentLifestyle.Singleton;

                if (component.HasAttribute<ComponentAttribute>())
                    lifestyle = component.GetAttribute<ComponentAttribute>().Lifestyle;

                // register all types that inherit the component type

                var implementationTypes = (from asm in assemblies
                                           from type in asm.GetTypes()
                                           where component.IsAssignableFrom(type)
                                           where type.IsClass && !type.IsAbstract
                                           select type);

                foreach (var implementation in implementationTypes)
                {
                    _resolver.Register(component, implementation, lifestyle);
                }
            }
        }

        public static void SetResolver(IDependencyResolver resolver)
        {
            if(resolver == null)
                throw new ArgumentNullException("resolver");

            _resolver = resolver;
        }

        public static IDependencyResolver Resolver
        {
            get
            {
                if(_resolver == null)
                    throw new ArgumentNullException();

                return _resolver;
            }
        }
    }
}
