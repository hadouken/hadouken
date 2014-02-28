using Nancy;

namespace Hadouken.Http.Management.Modules
{
    public class RepositoryModule : NancyModule
    {
        public RepositoryModule()
            : base("repository")
        {
            Get["/"] = _ => View["Index"];
        }
    }
}
