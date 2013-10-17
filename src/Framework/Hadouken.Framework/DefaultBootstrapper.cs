using System;
using System.Linq;
using Autofac;
using Hadouken.Framework.DI;
using Hadouken.Framework.IO;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.TypeScript;

namespace Hadouken.Framework
{
    public class DefaultBootstrapper : Bootstrapper
    {
        public override void RegisterFrameworkComponents(ContainerBuilder builder)
        {
            builder.RegisterAssemblyModules<ParameterlessConstructorModule>(typeof (Bootstrapper).Assembly);
            builder.RegisterModule(new ConfigModule(Configuration));
            builder.RegisterType<PluginManagerService>().As<IPluginManagerService>();
            builder.RegisterType<PluginHost>().As<IPluginHost>().SingleInstance();
            builder.RegisterType<RootPathProvider>().As<IRootPathProvider>().SingleInstance();
            // TypeScript
            builder.RegisterType<TypeScriptCompiler>().As<ITypeScriptCompiler>().SingleInstance();
        }

        public override void RegisterPlugin(ContainerBuilder builder)
        {
            // Find the plugin type
            var pluginType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from type in asm.GetTypes()
                where typeof (Plugin).IsAssignableFrom(type)
                where type.IsClass && !type.IsAbstract
                select type).First();

            if (pluginType == null)
                throw new Exception("No plugin found");

            builder.RegisterType(pluginType).As<Plugin>().SingleInstance();
        }
    }
}