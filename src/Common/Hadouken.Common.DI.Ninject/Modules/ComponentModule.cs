using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ninject.Extensions.Conventions;
using Ninject.Infrastructure.Language;
using Ninject.Modules;

namespace Hadouken.Common.DI.Ninject.Modules
{
    public class ComponentModule : NinjectModule
    {
        public override void Load()
        {
            var componentTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                  from type in asm.GetTypes()
                                  where type.HasAttribute<ComponentAttribute>()
                                  where type.IsClass && !type.IsAbstract
                                  let attr = type.GetAttribute<ComponentAttribute>()
                                  select new
                                      {
                                          Type = type,
                                          Attribute = attr
                                      });

            foreach (var componentType in componentTypes)
            {
                switch (componentType.Attribute.ComponentType)
                {
                    case ComponentType.Singleton:
                        Kernel.Bind(componentType.Type.GetInterfaces()).To(componentType.Type).InSingletonScope();
                        break;

                    case ComponentType.Transient:
                        Kernel.Bind(componentType.Type.GetInterfaces()).To(componentType.Type).InTransientScope();
                        break;
                }
            }
        }
    }

    public static class ReflectionExtensions
    {
        public static T GetAttribute<T>(this MemberInfo mi)
        {
            return (T)mi.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
