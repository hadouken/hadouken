using Autofac;
using Nancy.Authentication.Basic;
using Nancy.Authentication.Forms;

namespace Hadouken.Http.Management
{
    public class ManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ManagementServer>().As<IManagementServer>();
            builder.RegisterType<HadoukenUserMapper>().As<IUserMapper>();
            builder.RegisterType<HadoukenUserValidator>().As<IUserValidator>();
        }
    }
}
