using Nancy;
using Nancy.Security;

namespace Hadouken.Http.Management.Modules
{
    public abstract class ModuleBase : NancyModule
    {
        protected ModuleBase()
            : this("")
        {
        }

        protected ModuleBase(string modulePath)
            : base(modulePath)
        {
            this.RequiresAuthentication();
        }
    }
}