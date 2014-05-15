namespace Hadouken.Http.Management.Modules
{
    public class HomeModule : ModuleBase
    {
        public HomeModule()
        {
            Get["/"] = _ => View["Index"];
        }
    }
}
