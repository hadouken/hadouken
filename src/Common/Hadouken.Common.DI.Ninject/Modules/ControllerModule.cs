using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Hadouken.Common.Http.Mvc;

namespace Hadouken.Common.DI.Ninject.Modules
{
    public class ControllerModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(ctx => ctx.From(AppDomain.CurrentDomain.GetAssemblies())
                                  .SelectAllClasses()
                                  .InheritedFrom<IController>()
                                  .BindAllInterfaces());
        }
    }
}
