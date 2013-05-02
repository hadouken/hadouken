using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.Plugins;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace Hadouken.Common.DI.Ninject.Modules
{
    public class PluginModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(ctx =>
                        ctx.From(AppDomain.CurrentDomain.GetAssemblies())
                           .SelectAllClasses()
                           .InheritedFrom<Plugin>()
                           .BindBase());
        }
    }
}
