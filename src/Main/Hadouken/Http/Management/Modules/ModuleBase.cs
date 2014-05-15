using Nancy;

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
            Before += (ctx) =>
            {
                if (ctx.Request.UserHostAddress == "127.0.0.1"
                    || ctx.Request.UserHostAddress == "::1"
                    || ctx.CurrentUser != null)
                {
                    return null;
                }

                return HttpStatusCode.Unauthorized;
            };
        }
    }
}