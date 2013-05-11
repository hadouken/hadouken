using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Reflection;
using Ninject.Modules;
using Hadouken.Http;

namespace Hadouken.DI.Ninject.Modules
{
    public class ApiActionModule : NinjectModule
    {
        public override void Load()
        {
            var apiActionTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
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
