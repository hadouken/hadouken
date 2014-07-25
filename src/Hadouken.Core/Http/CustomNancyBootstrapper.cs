using System;
using Autofac;
using Hadouken.Core.Http.Security;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;

namespace Hadouken.Core.Http
{
    public class CustomNancyBootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILifetimeScope _lifetimeScope;

        public CustomNancyBootstrapper(ILifetimeScope lifetimeScope)
        {
            if (lifetimeScope == null) throw new ArgumentNullException("lifetimeScope");
            _lifetimeScope = lifetimeScope;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _lifetimeScope;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            var tokenizer = container.Resolve<ITokenizer>();
            var cfg = new TokenAuthenticationConfiguration(tokenizer);

            TokenAuthentication.Enable(pipelines, cfg);
        }
    }
}
