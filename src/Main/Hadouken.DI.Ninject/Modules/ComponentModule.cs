using Hadouken.Common;
using Hadouken.Reflection;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ninject.Extensions.Conventions;

namespace Hadouken.DI.Ninject.Modules
{
    public class ComponentModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(
                x =>
                x.From(AppDomain.CurrentDomain.GetAssemblies())
                 .Select(t => t.HasAttribute<ComponentAttribute>())
                 .BindAllInterfaces());
        }
    }
}
