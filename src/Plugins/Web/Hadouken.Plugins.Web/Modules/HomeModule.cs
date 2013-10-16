using Nancy;

namespace Hadouken.Plugins.WebUI.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = _ => Response.AsFile("UI/index.html");
        }
    }
}
