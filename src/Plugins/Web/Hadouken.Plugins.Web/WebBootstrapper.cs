using System;
using Autofac;
using Hadouken.Framework;
using Hadouken.Framework.Events;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Web.Http;

namespace Hadouken.Plugins.Web
{
    public class WebBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<HttpFileServer>()
                .As<IHttpFileServer>()
                .WithParameter("listenUri", "http://localhost:7890/")
                .SingleInstance();

            containerBuilder.Register<IHttpFileServer>(c =>
            {
                var eventListener = c.Resolve<IEventListener>();
                var listenUri = String.Format("http://{0}:{1}/", Configuration.HostBinding, Configuration.Port);
                var rpcClient = c.Resolve<IJsonRpcClient>();

                var server = new HttpFileServer(listenUri, rpcClient, eventListener);
                server.SetCredentials(Configuration.UserName, Configuration.Password);

                return server;
            });
        }
    }
}
