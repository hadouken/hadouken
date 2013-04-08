using System;
using System.Collections.Generic;
using System.Linq;
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
            Kernel.Bind(
                x =>
                x.From(AppDomain.CurrentDomain.GetAssemblies())
                 .Select(t => t.HasAttribute<ComponentAttribute>())
                 .BindAllInterfaces());
        }
    }
}
