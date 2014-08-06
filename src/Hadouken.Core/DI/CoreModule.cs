using Autofac;
using Hadouken.Common.JsonRpc;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.Handlers;
using Hadouken.Core.Http;
using Hadouken.Core.Http.Security;
using Hadouken.Core.JsonRpc;
using Hadouken.Core.Services;
using Nancy.Bootstrapper;
using Ragnar;

namespace Hadouken.Core.DI
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // BitTorrent stuff
            builder.RegisterType<Session>().As<ISession>().SingleInstance().ExternallyOwned();
            builder.RegisterType<SessionHandler>().AsImplementedInterfaces().SingleInstance();

            // Extensions
            builder.RegisterType<ExtensionFactory>().As<IExtensionFactory>().SingleInstance();
            builder.RegisterType<NotifierHandler>().As<INotifierHandler>().SingleInstance();

            // JSONRPC host
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<JsonRpcRequestParser>().As<IJsonRpcRequestParser>().SingleInstance();
            builder.RegisterType<MethodCacheBuilder>().As<IMethodCacheBuilder>();
            builder.RegisterType<ByNameResolver>().As<IParameterResolver>();
            builder.RegisterType<ByPositionResolver>().As<IParameterResolver>();
            builder.RegisterType<NullResolver>().As<IParameterResolver>();

            // JSONRPC services
            builder.RegisterType<BitTorrentService>().As<IJsonRpcService>();
            builder.RegisterType<ConfigurationService>().As<IJsonRpcService>();
            builder.RegisterType<ExtensionService>().As<IJsonRpcService>();

            // Message handlers
            builder.RegisterType<NotifyTorrentAddedHandler>().AsImplementedInterfaces();
            builder.RegisterType<NotifyTorrentCompletedHandler>().AsImplementedInterfaces();

            // HTTP
            builder.RegisterType<HttpServer>().As<IHttpServer>().SingleInstance();
            builder.RegisterType<CustomNancyBootstrapper>().As<INancyBootstrapper>().SingleInstance();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().SingleInstance();
            builder.RegisterType<UserManager>().As<IUserManager>().SingleInstance();

            // The main service
            builder.RegisterType<HadoukenService>().As<IHadoukenService>().SingleInstance();
        }
    }
}
