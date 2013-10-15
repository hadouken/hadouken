using Autofac;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;

namespace Hadouken.Framework.DI
{
    public class JsonRpcServiceModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceTypes = AppDomainExplorer.TypesInheritedFrom<IJsonRpcService>();
            builder.RegisterTypes(serviceTypes).AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<WcfJsonRpcService>().As<IWcfRpcService>();
        }
    }
}
