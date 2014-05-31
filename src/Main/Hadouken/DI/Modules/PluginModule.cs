using Autofac;
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;

namespace Hadouken.DI.Modules
{
    public sealed class PluginModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PluginManagerFactory>().As<IPluginManagerFactory>();
            builder.RegisterType<IsolatedEnvironmentFactory>().As<IIsolatedEnvironmentFactory>();
            builder.RegisterType<DevelopmentPluginInstaller>().As<IDevelopmentPluginInstaller>();
            builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();
        }
    }
}
