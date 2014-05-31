using System.Reflection;
using Autofac;
using Hadouken.Http.Security;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.ViewEngines;

namespace Hadouken.Http.Management
{
    public class CustomNancyBootstrapper : AutofacNancyBootstrapper
    {

        private readonly ILifetimeScope _container;
        private readonly Assembly _assembly;

        public CustomNancyBootstrapper(ILifetimeScope container)
        {
            _container = container;
            _assembly = GetType().Assembly;

            StaticConfiguration.DisableErrorTraces = false;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope container)
        {
            base.ConfigureApplicationContainer(container);
            ResourceViewLocationProvider.RootNamespaces.Add(_assembly, "Hadouken.Http.Management.UI");
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            var tokenizer = container.Resolve<ITokenizer>();
            var tokenConfiguration = new TokenAuthenticationConfiguration(tokenizer);
            
            TokenAuthentication.Enable(pipelines, tokenConfiguration);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            var namespaceRoot = "Hadouken.Http.Management.UI";

            var css = EmbeddedStaticContentConventionBuilder.AddDirectory("/css", _assembly,
                namespaceRoot: namespaceRoot);
            var fonts = EmbeddedStaticContentConventionBuilder.AddDirectory("/fonts", _assembly,
                namespaceRoot: namespaceRoot);
            var img = EmbeddedStaticContentConventionBuilder.AddDirectory("/img", _assembly,
                namespaceRoot: namespaceRoot);
            var js = EmbeddedStaticContentConventionBuilder.AddDirectory("/js", _assembly,
                 namespaceRoot: namespaceRoot);
            var views = EmbeddedStaticContentConventionBuilder.AddDirectory("/views", _assembly,
                namespaceRoot: namespaceRoot);

            nancyConventions.StaticContentsConventions.Add(css);
            nancyConventions.StaticContentsConventions.Add(fonts);
            nancyConventions.StaticContentsConventions.Add(img);
            nancyConventions.StaticContentsConventions.Add(js);
            nancyConventions.StaticContentsConventions.Add(views);
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
