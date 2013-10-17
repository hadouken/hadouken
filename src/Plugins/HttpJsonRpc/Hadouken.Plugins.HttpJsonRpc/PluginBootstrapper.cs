using System;
using Autofac;
using Hadouken.Framework;
using Hadouken.Framework.Events;

namespace Hadouken.Plugins.HttpJsonRpc
{
    public class PluginBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.Register<IHttpJsonRpcServer>(c =>
            {
                var eventListener = c.Resolve<IEventListener>();

                var uri = String.Format("http://{0}:{1}/jsonrpc/", Configuration.HostBinding, Configuration.Port);

                var server = new HttpJsonRpcServer(uri, eventListener);
                server.SetCredentials(Configuration.UserName, Configuration.Password);

                return server;
            });
        }
    }
}
