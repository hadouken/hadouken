using Nancy;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.Hosting.Self;

namespace Hadouken.Plugins.WebUI
{
    public class CustomNancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new FileSystemRootPathProvider(); }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", "UI/", "html", "css"));
        }
    }
}
