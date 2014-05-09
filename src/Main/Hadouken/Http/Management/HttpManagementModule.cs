using Autofac;
using Hadouken.Http.Management.Security;
using Nancy.Authentication.Forms;

namespace Hadouken.Http.Management
{
    public sealed class HttpManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserMapper>().As<IUserMapper>();
            builder.RegisterType<HttpBackendServer>().As<IHttpBackendServer>();
        }
    }
}
