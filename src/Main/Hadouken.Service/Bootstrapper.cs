using Autofac;
using System;
using Hadouken.Configuration;
using Hadouken.Configuration.AppConfig;
using Hadouken.Framework.DI;
using Hadouken.Framework.Events;
using Hadouken.Framework.IO;
using Hadouken.Framework.IO.Local;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Wcf;
using Hadouken.Gateway;
using Hadouken.Http;
using Hadouken.Http.Management;
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Rpc;
using Hadouken.Rpc;
using Hadouken.Events;
using Hadouken.Events.Rpc;

namespace Hadouken.Service
{
	public sealed class Bootstrapper
	{
		public IHadoukenService Build()
		{
			var builder = new ContainerBuilder();

			// Register service
			builder.RegisterType<HadoukenService>().As<IHadoukenService>();

            // HTTP management interface
		    builder.RegisterModule(new ManagementModule());

			// Register plugin engine
		    builder.RegisterType<PackageDownloader>().As<IPackageDownloader>();
		    builder.RegisterType<PackageFactory>().As<IPackageFactory>();
		    builder.RegisterType<IsolatedEnvironmentFactory>().As<IIsolatedEnvironmentFactory>();
			builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();

			// Register file system
			builder.RegisterType<LocalFileSystem>().As<IFileSystem>().SingleInstance();
		    builder.RegisterType<RootPathProvider>().As<IRootPathProvider>().SingleInstance();

			// Register RPC services
            builder.RegisterType<PluginsService>().As<IJsonRpcService>();
            builder.RegisterType<EventsService>().As<IJsonRpcService>();
		    builder.RegisterType<CoreServices>().As<IJsonRpcService>();
			builder.RegisterType<WcfProxyRequestHandler>().As<IRequestHandler>().SingleInstance();
			builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();

		    builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();

		    builder.RegisterGeneric(typeof (ProxyFactory<>)).As(typeof (IProxyFactory<>)).SingleInstance();

		    builder.RegisterType<WcfClientTransport>()
		        .As<IClientTransport>()
		        .WithParameter("endpoint", new Uri("net.pipe://localhost/hdkn.jsonrpc"));

            // Register SignalR event server
		    builder.RegisterType<EventServer>().As<IEventServer>().SingleInstance();

            // API connection
		    builder.RegisterType<ApiConnection>().As<IApiConnection>();

		    builder.Register<IEventListener>(c =>
		    {
		        var conf = c.Resolve<IConfiguration>();
		        var eventListenerUri = String.Format("http://{0}:{1}/events", conf.Http.HostBinding, conf.Http.Port);

		        return new EventListener(eventListenerUri);
		    });

			// Register configuration
			builder.Register(c => HadoukenConfigurationSection.Load()).SingleInstance();

		    builder.RegisterType<GatewayPluginManagerService>().As<IPluginManagerService>();
			builder.RegisterModule(new ServiceHostFactoryModule());

			// Resolve the service.
		    var container = builder.Build();
			return container.Resolve<IHadoukenService>();
		}
	}
}
