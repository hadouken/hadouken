using Autofac;
using Hadouken.Core.BitTorrent;
using Ragnar;

namespace Hadouken.Core.DI
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Session>().As<ISession>().SingleInstance();
            builder.RegisterType<Service>().As<IService>().SingleInstance();
            builder.RegisterType<SessionHandler>().As<ISessionHandler>();
        }
    }
}
