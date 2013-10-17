using Autofac;
using Hadouken.Framework.Wcf;

namespace Hadouken.Framework.DI
{
    public class ServiceHostFactoryModule : ParameterlessConstructorModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BindingFactory>()
                .As<IBindingFactory>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(ServiceHostFactory<>))
                .As(typeof(IServiceHostFactory<>));
        }
    }
}
