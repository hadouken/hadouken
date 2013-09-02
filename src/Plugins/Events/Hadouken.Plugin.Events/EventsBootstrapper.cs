using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using InjectMe;
using InjectMe.Registration;

namespace Hadouken.Plugins.Events
{
    public class EventsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = BuildContainer(config);
            return container.ServiceLocator.Resolve<Plugin>();
        }

        private static IContainer BuildContainer(IBootConfig config)
        {
            return
                Container.Create(
                    containerConfiguration => BuildContainerConfiguration(containerConfiguration, config));
        }

        private static void BuildContainerConfiguration(IContainerConfiguration containerConfiguration, IBootConfig config)
        {
            containerConfiguration.Register<IEventServer>()
                .AsSingleton()
                .UsingFactory(() => new EventServer(config.HostBinding));

            containerConfiguration.Register<Plugin>().AsTransient().UsingConcreteType<EventsPlugin>();
        }
    }
}
