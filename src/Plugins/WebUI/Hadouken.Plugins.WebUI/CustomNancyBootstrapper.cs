using System;
using System.IO;
using Nancy;
using Nancy.Conventions;
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
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI");
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", "UI/"));
            
            base.ConfigureConventions(nancyConventions);
        }
    }
}
