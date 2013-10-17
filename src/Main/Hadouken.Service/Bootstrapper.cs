using Autofac;
using System;
using Hadouken.Configuration;
using Hadouken.Framework.DI;
using Hadouken.Framework.Events;
using Hadouken.Framework.IO;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.TypeScript;
using Hadouken.Framework.Wcf;
using Hadouken.Plugins;
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

			// Register plugin engine
			builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();

			// Register plugin loaders
			builder.RegisterType<DirectoryPluginLoader>().As<IPluginLoader>();

			// Register file system
			builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
		    builder.RegisterType<RootPathProvider>().As<IRootPathProvider>().SingleInstance();

            // TypeScript
		    builder.RegisterType<TypeScriptCompiler>().As<ITypeScriptCompiler>().SingleInstance();

			// Register RPC services
            builder.RegisterType<PluginsService>().As<IJsonRpcService>().SingleInstance();
            builder.RegisterType<EventsService>().As<IJsonRpcService>().SingleInstance();
		    builder.RegisterType<CoreServices>().As<IJsonRpcService>().SingleInstance();
			builder.RegisterType<WcfProxyRequestHandler>().As<IRequestHandler>().SingleInstance();
			builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();

		    builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();

		    builder.RegisterGeneric(typeof (ProxyFactory<>)).As(typeof (IProxyFactory<>)).SingleInstance();

		    builder.RegisterType<WcfClientTransport>()
		        .As<IClientTransport>()
		        .WithParameter("endpoint", new Uri("net.pipe://localhost/hdkn.jsonrpc"));

            // Register SignalR event server
		    builder.RegisterType<EventServer>().As<IEventServer>().SingleInstance();

		    builder.Register<IEventListener>(c =>
		    {
		        var conf = c.Resolve<IConfiguration>();
		        var eventListenerUri = String.Format("http://{0}:{1}/events", conf.Http.HostBinding, conf.Http.Port);

		        return new EventListener(eventListenerUri);
		    });

			// Register configuration
			builder.Register(c => ApplicationConfigurationSection.Load()).SingleInstance();

		    builder.RegisterType<PluginManagerService>().As<IPluginManagerService>();
			builder.RegisterModule(new ServiceHostFactoryModule());

			// Resolve the service.
		    var container = builder.Build();
			return container.Resolve<IHadoukenService>();
		}
	}
}
