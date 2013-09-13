using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Windows.Forms;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.WebApi;
using Hadouken.Configuration;
using Hadouken.Http;
using Hadouken.IO;
using Hadouken.Plugins;
using Hadouken.Plugins.Http.Controllers;

namespace Hadouken.Service
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            BuildContainer();

            var serviceHost = Container.Resolve<HostingService>();

            if (Bootstrapper.RunAsConsoleIfRequested(serviceHost))
                return;

            ServiceBase.Run(new ServiceBase[] {serviceHost});
        }

        private static void BuildContainer()
        {
            var builder = new ContainerBuilder();

            // Register service
            builder.RegisterType<DefaultHostingService>().As<HostingService>();

            // Register plugin engine
            builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();

            // Register plugin loaders
            builder.RegisterType<DirectoryPluginLoader>().As<IPluginLoader>();

            // Register file system
            builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();

            // Register configuration
            builder.Register(c => ApplicationConfigurationSection.Load()).SingleInstance();

            // Register Web API server
            builder.RegisterType<HttpWebApiServer>().As<IHttpWebApiServer>().SingleInstance();

            // Register controllers
            builder.RegisterApiControllers(typeof (PluginsController).Assembly);

            // Register dep resolver
            builder.Register<IDependencyResolver>(c => new AutofacWebApiDependencyResolver(Container));

            Container = builder.Build();
        }

        private static IContainer Container { get; set; }
    }
}
