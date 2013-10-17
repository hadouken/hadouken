using Autofac;
using Hadouken.Framework.Wcf;

namespace Hadouken.Framework.DI
{
    public class ProxyFactoryModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof (ProxyFactory<>))
                .As(typeof (IProxyFactory<>))
                .SingleInstance();
        }
    }
}
