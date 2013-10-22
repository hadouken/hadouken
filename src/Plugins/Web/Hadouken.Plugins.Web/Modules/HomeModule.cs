using Nancy;

namespace Hadouken.Plugins.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => Response.AsFile("UI/index.html");
        }
    }
}
