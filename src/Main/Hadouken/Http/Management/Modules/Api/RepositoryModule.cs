using Hadouken.Plugins.Repository;
using Nancy;

namespace Hadouken.Http.Management.Modules.Api
{
    public class RepositoryModule : NancyModule
    {
        public RepositoryModule(IPluginRepository pluginRepository)
            : base("api/repository")
        {
            Get["/"] = _ => Negotiate.WithModel(pluginRepository.GetAll());
        }
    }
}
