using System;
using System.IO;
using Nancy;
using Nancy.Conventions;
using Nancy.Hosting.Self;

namespace Hadouken.Plugins.WebUI.Nancy
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new FileSystemRootPathProvider(); }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI");
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", path));
            
            base.ConfigureConventions(nancyConventions);
        }
    }
}
