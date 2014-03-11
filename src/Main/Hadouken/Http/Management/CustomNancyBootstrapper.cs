using Autofac;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.ViewEngines;

namespace Hadouken.Http.Management
{
    public class CustomNancyBootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILifetimeScope _container;

        public CustomNancyBootstrapper(ILifetimeScope container)
        {
            _container = container;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            // On a per-request basis, look to see what kind of authentication we want.
            // We support both Basic Auth and Forms Auth.

            if (!string.IsNullOrEmpty(context.Request.Headers.Authorization)
                && context.Request.Headers.Authorization.StartsWith("Basic"))
            {
                var validator = container.Resolve<IUserValidator>();
                pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(validator, "Hadouken"));
            }
            else
            {
                FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration
                {
                    RedirectUrl = "~/login",
                    UserMapper = container.Resolve<IUserMapper>()
                });
            }
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            base.ConfigureApplicationContainer(container);

            var assembly = typeof (CustomNancyBootstrapper).Assembly;
            ResourceViewLocationProvider.RootNamespaces.Add(assembly, "Hadouken.Http.Management.UI");
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            var assembly = typeof(CustomNancyBootstrapper).Assembly;
            var content = EmbeddedStaticContentConventionBuilder.AddDirectory("/Content", assembly,
                namespaceRoot: "Hadouken.Http.Management.UI");
            var scripts = EmbeddedStaticContentConventionBuilder.AddDirectory("/Scripts", assembly,
                namespaceRoot: "Hadouken.Http.Management.UI");

            nancyConventions.StaticContentsConventions.Add(content);
            nancyConventions.StaticContentsConventions.Add(scripts);
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get { return NancyInternalConfiguration.WithOverrides(OnConfigurationBuilder); }
        }

        private void OnConfigurationBuilder(NancyInternalConfiguration obj)
        {
            obj.ViewLocationProvider = typeof (ResourceViewLocationProvider);
        }
    }
}
