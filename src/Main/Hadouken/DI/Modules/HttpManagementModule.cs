using Autofac;
using Hadouken.Http.Management;
using Hadouken.Http.Security;

namespace Hadouken.DI.Modules
{
    public sealed class HttpManagementModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Tokenizer>().As<ITokenizer>().SingleInstance();
            builder.RegisterType<HttpBackendServer>().As<IHttpBackendServer>();
        }
    }
}
