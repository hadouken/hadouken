using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hadouken.Reflection;
using System.Reflection;

using Ninject;

namespace Hadouken
{
    public static class Kernel
    {
        private static IKernel _kernel;

        public static void Register(params Assembly[] assemblies)
        {
            if (_kernel == null)
                _kernel = new StandardKernel();

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
                    Bind(component, implementation, lifestyle);
                }
            }
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return _kernel.GetAll<T>();
        }

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static object Get(Type t)
        {
            return _kernel.Get(t);
        }

        public static T Get<T>(string name)
        {
            return _kernel.Get<T>(name);
        }

        public static T TryGet<T>()
        {
            return _kernel.TryGet<T>();
        }

        public static T TryGet<T>(string name)
        {
            return _kernel.TryGet<T>(name);
        }

        public static void Bind(Type component, Type implementation, ComponentLifestyle lifestyle, string name)
        {
            switch (lifestyle)
            {
                case ComponentLifestyle.Singleton:
                    _kernel.Bind(component).To(implementation).InSingletonScope().Named(name);
                    break;

                case ComponentLifestyle.Transient:
                    _kernel.Bind(component).To(implementation).InTransientScope().Named(name);
                    break;
            }
        }

        public static void Bind(Type component, Type implementation, ComponentLifestyle lifestyle)
        {
            switch (lifestyle)
            {
                case ComponentLifestyle.Singleton:
                    _kernel.Bind(component).To(implementation).InSingletonScope();
                    break;

                case ComponentLifestyle.Transient:
                    _kernel.Bind(component).To(implementation).InTransientScope();
                    break;
            }
        }
    }
}
