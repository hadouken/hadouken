using Autofac;
using Nancy.Authentication.Forms;

namespace Hadouken.Http.Management
{
    public class ManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ManagementServer>().As<IManagementServer>();
            builder.RegisterType<HadoukenUserMapper>().As<IUserMapper>();
        }
    }
}
