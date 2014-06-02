using System.IO;
using System.IO.Compression;
using Autofac;
using Hadouken.Configuration;
using Hadouken.Fx.IO;
using Hadouken.Http.Security;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Responses;

namespace Hadouken.Http.Management
{
    public class CustomNancyBootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILifetimeScope _container;
        public CustomNancyBootstrapper(ILifetimeScope container)
        {
            _container = container;

            StaticConfiguration.DisableErrorTraces = false;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _container;
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            var tokenizer = container.Resolve<ITokenizer>();
            var tokenConfiguration = new TokenAuthenticationConfiguration(tokenizer);
            
            TokenAuthentication.Enable(pipelines, tokenConfiguration);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            var cfg = _container.Resolve<IConfiguration>();
            var fileSystem = _container.Resolve<IFileSystem>();

            nancyConventions.StaticContentsConventions.Add((context, s) =>
            {
                var requestedFile = (context.Request.Path == "/" ? "/index.html" : context.Request.Path);
                if (requestedFile.StartsWith("/")) requestedFile = requestedFile.Substring(1);

                if (cfg.WebApplicationPath.EndsWith(".zip"))
                {
                    using (var archive = ZipFile.Open(cfg.WebApplicationPath, ZipArchiveMode.Read))
                    {
                        var entry = archive.GetEntry(requestedFile);
                        
                        if (entry == null)
                        {
                            return null;
                        }

                        var ms = new MemoryStream();
                        entry.Open().CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        return new StreamResponse(() => ms, MimeTypes.GetMimeType(requestedFile));
                    }
                }
                
                var filePath = Path.Combine(cfg.WebApplicationPath, requestedFile);
                var file = fileSystem.GetFile(filePath);

                if (!file.Exists)
                {
                    return null;
                }

                return new StreamResponse(file.OpenRead, MimeTypes.GetMimeType(filePath));
            });
        }
    }
}
