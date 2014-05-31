using Autofac;
using Hadouken.Configuration;
using Hadouken.Fx.JsonRpc;

namespace Hadouken.DI.Modules
{
    public sealed class JsonRpcModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JsonRpcRequestParser>().As<IJsonRpcRequestParser>();
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<MethodCacheBuilder>().As<IMethodCacheBuilder>();
            builder.RegisterType<ByNameResolver>().As<IParameterResolver>();
            builder.RegisterType<ByPositionResolver>().As<IParameterResolver>();
            builder.RegisterType<NullResolver>().As<IParameterResolver>();
            builder.RegisterType<JsonSerializer>().As<IJsonSerializer>();
            builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();
            builder.Register<IClientTransport>(c =>
            {
                var cfg = c.Resolve<IConfiguration>();
                return new WcfClientTransport(cfg.PluginUrlTemplate, c.Resolve<IJsonSerializer>());
            });
        }
    }
}
