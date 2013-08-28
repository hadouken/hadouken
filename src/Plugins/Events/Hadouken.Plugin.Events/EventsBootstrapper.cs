using Domo.DI;
using Domo.DI.Registration;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.Events
{
    public class EventsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = Container.Create(_ => RegisterComponents(config, _));
            return container.ServiceLocator.Resolve<Plugin>();
        }

        private static void RegisterComponents(IBootConfig bootConfig, IContainerConfiguration cfg)
        {
            cfg.Register<IEventServer>().AsSingleton().UsingFactory(() => new EventServer(bootConfig.HostBinding));
            cfg.Register<Plugin>().AsTransient().UsingConcreteType<EventsPlugin>();
        }
    }
}
