using System.Linq;
using System.Reflection;
using Autofac;
using Hadouken.Plugins;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.ViewEngines;

namespace Hadouken.Http.Management
{
    public class CustomNancyBootstrapper : AutofacNancyBootstrapper
    {
#pragma warning disable 169
        private static readonly Nancy.ViewEngines.Razor.DefaultRazorConfiguration conf__;
        private static readonly System.Web.Razor.RazorCodeLanguage lang__;
#pragma warning restore 169

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

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            FormsAuthentication.Enable(pipelines, new FormsAuthenticationConfiguration
            {
                RedirectUrl = "~/login",
                UserMapper = container.Resolve<IUserMapper>()
            });

            pipelines.AfterRequest.AddItemToEndOfPipeline(context =>
            {
                var engine = _container.Resolve<IPluginEngine>();
                var pluginsBackgroundScripts =
                    engine.GetAll()
                        .Where(
                            p =>
                                p.Manifest.UserInterface != null && p.Manifest.UserInterface.BackgroundScripts.Any());


                // Get all background scripts
                var scripts = (from plugin in pluginsBackgroundScripts
                               from bgs in plugin.Manifest.UserInterface.BackgroundScripts
                               select string.Concat(plugin.Manifest.Name, "/", bgs)).ToArray();

                // Get all plugins which have settings to show
                var settingsPages = (from plugin in engine.GetAll()
                                     where plugin.Manifest.UserInterface != null
                                     let pages = plugin.Manifest.UserInterface.Pages
                                     from page in pages
                                     where page.Key == "settings"
                                     select plugin.Manifest.Name).ToArray();

                context.ViewBag.BackgroundScripts = scripts;
                context.ViewBag.SettingsPages = settingsPages;
                context.ViewBag.IsLocal = (context.Request.UserHostAddress == "127.0.0.1"
                                           || context.Request.UserHostAddress == "::1");
            });
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            var content = EmbeddedStaticContentConventionBuilder.AddDirectory("/Content", _assembly,
                namespaceRoot: "Hadouken.Http.Management.UI");
            var scripts = EmbeddedStaticContentConventionBuilder.AddDirectory("/Scripts", _assembly,
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
