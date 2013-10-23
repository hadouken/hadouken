using System;
using System.IO;
using System.Text;
using Hadouken.Plugins.Web.CoffeeScript;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;

namespace Hadouken.Plugins.Web
{
    public class CustomNancyBootstrapper : DefaultNancyBootstrapper
    {
        protected override IRootPathProvider RootPathProvider
        {
            get { return new FileSystemRootPathProvider(); }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.AfterRequest.AddItemToEndOfPipeline(
                ctx => CompileCoffeeFiles(container.Resolve<ICoffeeCompiler>(), ctx));
        }

        private static void CompileCoffeeFiles(ICoffeeCompiler compiler, NancyContext context)
        {
            if (!context.Request.Path.EndsWith(".coffee"))
                return;

            if (context.Response.StatusCode != HttpStatusCode.OK)
                return;

            using (var ms = new MemoryStream())
            {
                context.Response.Contents(ms);
                var responseContent = Encoding.UTF8.GetString(ms.ToArray());

                if (String.IsNullOrEmpty(responseContent))
                {
                    context.Response.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    context.Response.ContentType = "text/javascript";
                    context.Response.Contents = stream =>
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            var compiledCoffeeResponse = compiler.Compile(responseContent);
                            writer.Write(compiledCoffeeResponse);
                        }
                    };
                }
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("/", "UI/", "html", "css", "js"));
        }
    }
}
