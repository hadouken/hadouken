using Autofac;
using Hadouken.Framework;
using Hadouken.Plugins.Web.Http;

namespace Hadouken.Plugins.Web
{
    public class WebBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder containerBuilder)
        {
            containerBuilder
                .RegisterType<HttpFileServer>()
                .As<IHttpFileServer>()
                .WithParameter("listenUri", "http://localhost:7890/")
                .SingleInstance();
        }
    }
}
