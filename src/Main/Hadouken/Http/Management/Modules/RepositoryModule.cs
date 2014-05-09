namespace Hadouken.Http.Management.Modules
{
    public class RepositoryModule : ModuleBase
    {
        public RepositoryModule()
            : base("repository")
        {
            Get["/"] = _ => View["Index"];
        }
    }
}
