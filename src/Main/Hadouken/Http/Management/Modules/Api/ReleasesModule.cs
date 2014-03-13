using Hadouken.Http.Api;
using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules.Api
{
    public class ReleasesModule : NancyModule
    {
        public ReleasesModule(IReleasesRepository releasesRepository)
            : base("api/releases")
        {
            this.RequiresAuthentication();

            Get["/"] = _ => Negotiate.WithModel(releasesRepository.GetAll());
        }
    }
}