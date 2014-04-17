using Hadouken.Http.Api;
using Hadouken.Plugins;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules.Api
{
    public class RepositoryModule : NancyModule
    {
        public RepositoryModule(IPluginRepository pluginRepository, IPackageDownloader packageDownloader, IPluginEngine pluginEngine)
            : base("api/repository")
        {
            Get["/"] = _ => Negotiate.WithModel(pluginRepository.GetAll());

            Get["/{id}"] = _ =>
            {
                string id = _.id;
                return Negotiate.WithModel(pluginRepository.GetById(id));
            };

            Post["/install"] = _ =>
            {
                string id = Request.Form.id;
                var package = packageDownloader.Download(id);

                if (package == null)
                {
                    return Response.AsJson(new {result = false, message = "Error when downloading package."});
                }

                var installResult = pluginEngine.InstallOrUpgrade(package);

                if (!installResult)
                {
                    return Response.AsJson(new {result = false, message = "Failed to install package."});
                }

                return Response.AsJson(new {result = true});
            };
        }
    }
}
