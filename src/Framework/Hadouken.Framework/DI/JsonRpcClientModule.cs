using Autofac;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.DI
{
    /// <summary>
    /// Registers the JsonRpcClient and the WcfNamedPipeClientTransport.
    /// </summary>
    public class JsonRpcClientModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<WcfClientTransport>()
                .WithParameter(
                    (param, ctx) => param.Name == "rpcHost",
                    (param, ctx) => BuildRpcHostUri(ctx.Resolve<IBootConfig>()))
                .AsImplementedInterfaces();

            builder.RegisterType<JsonRpcClient>().AsImplementedInterfaces();
        }

        private string BuildRpcHostUri(IBootConfig config)
        {
            return config.RpcGatewayUri;
        }
    }
}
