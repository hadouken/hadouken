using Autofac;
using Hadouken.Security;
using Hadouken.Service;
using Hadouken.Startup;

namespace Hadouken.DI
{
    public sealed class Bootstrapper
    {
        public IHadoukenService Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(GetType().Assembly);

            // Register service
            builder.RegisterType<HadoukenService>().As<IHadoukenService>();

            // Startup tasks
            builder.RegisterType<PluginBootstrapperTask>().As<IStartupTask>();
            
            // Security
            builder.RegisterType<AuthenticationManager>().As<IAuthenticationManager>();

            // Resolve the service.
            var container = builder.Build();
            return container.Resolve<IHadoukenService>();
        }
    }
}
