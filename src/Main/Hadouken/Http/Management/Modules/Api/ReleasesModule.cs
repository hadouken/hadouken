using Hadouken.Http.Api;
using Nancy;

namespace Hadouken.Http.Management.Modules.Api
{
    public class ReleasesModule : NancyModule
    {
        public ReleasesModule(IReleasesRepository releasesRepository)
            : base("api/releases")
        {
            Get["/"] = _ => Negotiate.WithModel(releasesRepository.GetAll());
        }
    }
}