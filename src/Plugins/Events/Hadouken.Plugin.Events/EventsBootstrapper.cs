using System;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Http;
using Hadouken.Framework.Rpc.Wcf;
using Hadouken.Plugins.Events.Rpc;
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

        private static void BuildContainerConfiguration(IContainerConfiguration cfg, IBootConfig config)
        {
            string baseUri = String.Concat("http://", config.HostBinding, ":", config.Port);

            cfg.Register<IEventServer>().AsSingleton().UsingFactory(() => new EventServer(baseUri));

            cfg.Register<IJsonRpcServer>().AsTransient().UsingConcreteType<WcfJsonRpcServer>();
            cfg.Register<IUriFactory>()
                .AsTransient()
                .UsingFactory(() => new UriFactory("net.pipe://localhost/hdkn.rpc.events"));

            cfg.Register<IRequestBuilder>().AsTransient().UsingConcreteType<RequestBuilder>();

            cfg.Register<IRequestHandler>().AsTransient().UsingConcreteType<RequestHandler>();

            // Register RPC methods
            cfg.Register<IRpcMethod>().AsTransient().UsingFactory(() => new EventsPublish(baseUri + "/events"));

            cfg.Register<Plugin>().AsTransient().UsingConcreteType<EventsPlugin>();
        }
    }
}
