using System;
using Autofac;
using Hadouken.Framework.Events;

namespace Hadouken.Framework.DI
{
    public class EventListenerModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventListener>()
                .WithParameter(
                    (param, ctx) => param.Name == "host",
                    (param, ctx) => BuildEventServerUri(ctx.Resolve<IBootConfig>()))
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        private string BuildEventServerUri(IBootConfig config)
        {
            return String.Format("http://{0}:{1}/events", config.HostBinding, config.Port);
        }
    }
}
