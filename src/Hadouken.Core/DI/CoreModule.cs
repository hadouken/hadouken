using Autofac;
using Hadouken.Core.BitTorrent;
using Hadouken.Core.Http;
using Hadouken.Core.Http.Security;
using Hadouken.Core.JsonRpc;
using Nancy.Bootstrapper;
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

            // JSONRPC host
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<JsonRpcRequestParser>().As<IJsonRpcRequestParser>().SingleInstance();
            builder.RegisterType<MethodCacheBuilder>().As<IMethodCacheBuilder>();
            builder.RegisterType<ByNameResolver>().As<IParameterResolver>();
            builder.RegisterType<ByPositionResolver>().As<IParameterResolver>();
            builder.RegisterType<NullResolver>().As<IParameterResolver>();

            // HTTP
            builder.RegisterType<HttpServer>().As<IHttpServer>().SingleInstance();
            builder.RegisterType<CustomNancyBootstrapper>().As<INancyBootstrapper>().SingleInstance();
            builder.RegisterType<Tokenizer>().As<ITokenizer>().SingleInstance();
        }
    }
}
