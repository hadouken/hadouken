using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.DI
{
    /// <summary>
    /// Registers the JsonRpcClient and the WcfNamedPipeClientTransport.
    /// </summary>
    public class JsonRpcClientModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<WcfNamedPipeClientTransport>()
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
