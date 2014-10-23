using System;
using System.IO;
using System.IO.Compression;
using Autofac;
using Hadouken.Common;
using Hadouken.Common.IO;
using Hadouken.Common.Logging;
using Hadouken.Core.Http.Security;
using Hadouken.Core.Security;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Autofac;
using Nancy.Conventions;
using Nancy.Responses;

namespace Hadouken.Core.Http
{
    public class CustomNancyBootstrapper : AutofacNancyBootstrapper
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<CustomNancyBootstrapper> _logger;
        private readonly IEnvironment _environment;
        private readonly IFileSystem _fileSystem;
        private byte[] _favicon;

        public CustomNancyBootstrapper(ILifetimeScope lifetimeScope,
            ILogger<CustomNancyBootstrapper> logger,
            IEnvironment environment,
            IFileSystem fileSystem)
        {
            if (lifetimeScope == null) throw new ArgumentNullException("lifetimeScope");
            if (logger == null) throw new ArgumentNullException("logger");
            if (environment == null) throw new ArgumentNullException("environment");
            if (fileSystem == null) throw new ArgumentNullException("fileSystem");

            _lifetimeScope = lifetimeScope;
            _logger = logger;
            _environment = environment;
            _fileSystem = fileSystem;
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return _lifetimeScope;
        }

        protected override byte[] FavIcon
        {
            get { return _favicon ?? (_favicon = LoadFavicon()); }
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            var userManager = container.Resolve<IUserManager>();
            var cfg = new TokenAuthenticationConfiguration(userManager);

            TokenAuthentication.Enable(pipelines, cfg);

            pipelines.OnError.AddItemToStartOfPipeline((context, exception) => 
            {
                _logger.Error(exception, "Error in HTTP pipeline.");
                return null;
            });
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            var webPath = _environment.GetWebApplicationPath();

            nancyConventions.StaticContentsConventions.Add((context, s) =>
            {
                var requestedFile = (context.Request.Path == "/" ? "/index.html" : context.Request.Path);
                if (requestedFile.StartsWith("/")) requestedFile = requestedFile.Substring(1);

                if (webPath.FullPath.EndsWith(".zip") && webPath is FilePath)
                {
                    var package = ((FilePath) webPath).MakeAbsolute(_environment);

                    using (var archive = ZipFile.Open(package.FullPath, ZipArchiveMode.Read))
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

                var filePath = ((DirectoryPath) webPath).CombineWithFilePath(requestedFile);
                var file = _fileSystem.GetFile(filePath);

                if (!file.Exists)
                {
                    return null;
                }

                return new StreamResponse(file.OpenRead, MimeTypes.GetMimeType(filePath.GetExtension()));
            });
        }

        private byte[] LoadFavicon()
        {
            using (var resource = typeof(IEnvironment).Assembly.GetManifestResourceStream("Hadouken.Common.Resources.ApplicationIcon.ico"))
            {
                if (resource == null) return null;

                var tempFavicon = new byte[resource.Length];
                resource.Read(tempFavicon, 0, (int)resource.Length);
                return tempFavicon;
            }
        }
    }
}
