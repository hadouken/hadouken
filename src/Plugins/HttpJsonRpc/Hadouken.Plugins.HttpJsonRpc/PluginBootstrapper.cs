using System;
using Autofac;
using Hadouken.Framework;
using Hadouken.Framework.Events;
using Hadouken.Framework.Plugins;

namespace Hadouken.Plugins.HttpJsonRpc
{
    public class PluginBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var builder = new ContainerBuilder();

            builder.Register<IEventListener>(c =>
            {
                var eventListenerUri =
                    new Uri(String.Format("http://{0}:{1}/events", config.HostBinding, config.Port));

                return new EventListener(eventListenerUri);
            });

            builder.Register<IHttpJsonRpcServer>(c =>
            {
                var eventListener = c.Resolve<IEventListener>();

                var uri = String.Format("http://{0}:{1}/jsonrpc/", config.HostBinding, config.Port);

                var server = new HttpJsonRpcServer(uri, eventListener);
                server.SetCredentials(config.UserName, config.Password);

                return server;
            });

            builder.RegisterType<HttpJsonRpcPlugin>().As<Plugin>();

            var container = builder.Build();
            return container.Resolve<Plugin>();
        }
    }
}
