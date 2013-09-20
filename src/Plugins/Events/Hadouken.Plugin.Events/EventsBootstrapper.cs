using System;
using Autofac;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Events.Rpc;

namespace Hadouken.Plugins.Events
{
    public class EventsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            BuildContainer(config);
            return Container.Resolve<Plugin>();
        }

        private static void BuildContainer(IBootConfig config)
        {
            int port = config.Port + 1;
            string baseUri = String.Concat("http://", config.HostBinding, ":", port + "/events");

            var builder = new ContainerBuilder();

            builder.RegisterType<EventsPlugin>().As<Plugin>();

            builder.Register<IEventServer>(c => new EventServer(baseUri)).SingleInstance();

            // Register WCF json rpc server
            builder.Register<IJsonRpcServer>(c =>
            {
                var handler = c.Resolve<IJsonRpcHandler>();
                return new WcfJsonRpcServer("net.pipe://localhost/hdkn.plugins.core.events", handler);
            });

            builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();
            builder.RegisterType<EventsService>().As<IJsonRpcService>().SingleInstance();

            Container = builder.Build();
        }

        private static IContainer Container { get; set; }
    }
}
