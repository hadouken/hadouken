using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Web.CoffeeScript;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.TinyIoc;

namespace Hadouken.Plugins.Web
{
    public class RootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return @"C:\tretton37";
        }
    }

    public class CustomNancyBootstrapper : DefaultNancyBootstrapper
    {
        private static readonly Regex PluginRegex = new Regex("^/plugins/(?<pluginId>[a-zA-Z0-9\\.]*?)/(?<path>.*)$");
        private readonly IJsonRpcClient _rpcClient;

        public CustomNancyBootstrapper(IJsonRpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new RootPathProvider(); }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);
            nancyConventions.StaticContentsConventions.Add(BuildStaticContentResponse);
        }

        private Response BuildStaticContentResponse(NancyContext nancyContext, string s)
        {
            var pluginId = "core.web";
            var path = "UI" + nancyContext.Request.Path;

            if (PluginRegex.IsMatch(nancyContext.Request.Path))
            {
                var match = PluginRegex.Match(nancyContext.Request.Path);

                pluginId = match.Groups["pluginId"].Value;
                path = "UI/" + match.Groups["path"].Value;
            }

            var fileContents = _rpcClient.CallAsync<byte[]>(
                "plugins.getFileContents",
                new[] {pluginId, path})
                .Result;

            if (fileContents == null)
                return new NotFoundResponse();

            var mimeType = MimeTypes.GetMimeType(nancyContext.Request.Path);

            if (nancyContext.Request.Path.EndsWith(".coffee"))
            {
                var compiler = new CoffeeCompiler();
                var coffeeScript = compiler.Compile(Encoding.UTF8.GetString(fileContents));

                mimeType = "text/javascript";
                fileContents = Encoding.UTF8.GetBytes(coffeeScript);
            }

            var streamFactory = new Func<MemoryStream>(() => new MemoryStream(fileContents));
            return new StreamResponse(streamFactory, mimeType);
        }
    }
}
