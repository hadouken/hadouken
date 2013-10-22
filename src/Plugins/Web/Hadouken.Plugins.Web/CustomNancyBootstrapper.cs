using System;
using System.IO;
using System.Text;
using Hadouken.Plugins.Web.CoffeeScript;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;

namespace Hadouken.Plugins.Web
{
    public class CustomNancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new AppDomainRootPathProvider(); }
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            pipelines.AfterRequest.AddItemToEndOfPipeline(ctx =>
            {
                if (!ctx.Request.Path.EndsWith(".coffee")
                    && ctx.Response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                var currentContents = "";
                using (var ms = new MemoryStream())
                {
                    ctx.Response.Contents.Invoke(ms);
                    currentContents = Encoding.UTF8.GetString(ms.ToArray());
                }

                if (String.IsNullOrEmpty(currentContents))
                {
                    ctx.Response.StatusCode = HttpStatusCode.NotFound;
                    return;
                }

                ctx.Response.ContentType = "text/javascript";

                ctx.Response.Contents = s =>
                {
                    using (var writer = new StreamWriter(s))
                    {
                        var compiler = container.Resolve<ICoffeeCompiler>();
                        writer.Write(compiler.Compile(currentContents));
                    }
                };
            });
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", "UI/", "html", "css"));
        }
    }

    public class AppDomainRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }

}
