using System;
using System.ServiceProcess;

using Autofac;

using Hadouken.Configuration;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.IO;
using Hadouken.Plugins;
using Hadouken.Plugins.Rpc;
using Hadouken.Rpc;
using NLog;

namespace Hadouken.Service
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            IContainer container = BuildContainer();

            var serviceHost = container.Resolve<HostingService>();

            if (Bootstrapper.RunAsConsoleIfRequested(serviceHost))
                return;

            ServiceBase.Run(new ServiceBase[] {serviceHost});
        }

        private static IContainer BuildContainer()
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

            // Register RPC services
            builder.RegisterType<PluginsService>().As<IJsonRpcService>();
            builder.RegisterType<WcfProxyRequestHandler>().As<IRequestHandler>();
            builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>();

            // Register JSONRPC server
            builder.Register<IJsonRpcServer>(c =>
            {
                var handler = c.Resolve<IJsonRpcHandler>();
                var conf = c.Resolve<IConfiguration>();
                var uri = String.Format("http://{0}:{1}/jsonrpc/", conf.Http.HostBinding, conf.Http.Port);

                return new HttpJsonRpcServer(uri, handler);
            });

            // Register configuration
            builder.Register(c => ApplicationConfigurationSection.Load()).SingleInstance();

            return builder.Build();
        }
    }
}
