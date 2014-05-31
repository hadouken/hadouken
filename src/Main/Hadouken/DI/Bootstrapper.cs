using Autofac;
using Hadouken.Http;
using Hadouken.Http.Api;
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
            
            // API connection
            builder.RegisterType<ApiConnection>().As<IApiConnection>();
            builder.RegisterType<ReleasesRepository>().As<IReleasesRepository>();
            
            // Security
            builder.RegisterType<AuthenticationManager>().As<IAuthenticationManager>();

            // Resolve the service.
            var container = builder.Build();
            return container.Resolve<IHadoukenService>();
        }
    }
}
