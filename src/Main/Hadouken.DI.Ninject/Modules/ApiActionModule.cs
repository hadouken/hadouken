using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Reflection;
using Ninject.Modules;
using Hadouken.Http;
using System.Reflection;

namespace Hadouken.DI.Ninject.Modules
{
    public class ApiActionModule : NinjectModule
    {
        private readonly IEnumerable<Assembly> _assemblies;
 
        public ApiActionModule() : this(AppDomain.CurrentDomain.GetAssemblies())
        {
        }

        public ApiActionModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        public override void Load()
        {
            var apiActionTypes = (from asm in _assemblies
                                  from type in asm.GetTypes()
                                  where typeof (IApiAction).IsAssignableFrom(type)
                                  where type.IsClass && !type.IsAbstract
                                  select type);

            foreach (var apiActionType in apiActionTypes)
            {
                Kernel.Bind<IApiAction>().To(apiActionType);
            }
        }
    }
}
