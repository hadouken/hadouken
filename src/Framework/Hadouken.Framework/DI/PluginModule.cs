using System.Linq;
using Autofac;
using Hadouken.Framework.Plugins;

namespace Hadouken.Framework.DI
{
    /// <summary>
    /// Looks for a concrete class which inherits <see cref="Plugin"/> and registers it as a singleton.
    /// </summary>
    public class PluginModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterPlugin(builder);
        }

        private static void RegisterPlugin(ContainerBuilder builder)
        {
            var pluginType = AppDomainExplorer.TypesInheritedFrom<Plugin>().First();
            if (pluginType == null)
                return;

            builder.RegisterType(pluginType).As<Plugin>().SingleInstance();
        }
    }
}
