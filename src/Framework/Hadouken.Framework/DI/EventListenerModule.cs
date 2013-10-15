using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Hadouken.Framework.Events;

namespace Hadouken.Framework.DI
{
    public class EventListenerModule : Module
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
