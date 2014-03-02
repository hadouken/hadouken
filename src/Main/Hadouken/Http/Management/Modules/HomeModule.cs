using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            this.RequiresAuthentication();

            Get["/"] = _ => View["Index"];
        }
    }
}
