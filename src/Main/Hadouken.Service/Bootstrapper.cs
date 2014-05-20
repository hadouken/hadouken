using System;
using System.IO;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using Hadouken.Configuration;
using Hadouken.Configuration.AppConfig;
using Hadouken.Fx.IO;
using Hadouken.Fx.JsonRpc;
using Hadouken.Fx.ServiceModel;
using Hadouken.Http;
using Hadouken.Http.Api;
using Hadouken.Http.Management;
using Hadouken.JsonRpc;
using Hadouken.Logging;
using Hadouken.Messaging;
using Hadouken.Plugins;
using Hadouken.Plugins.Handlers;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Messages;
using Hadouken.Plugins.Scanners;
using Hadouken.Security;
using Hadouken.Startup;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Hadouken.Service
{
	public sealed class Bootstrapper
	{
		public IHadoukenService Build()
		{
			var builder = new ContainerBuilder();

			// Register service
			builder.RegisterType<HadoukenService>().As<IHadoukenService>();

            // Startup tasks
            builder.RegisterType<PluginBootstrapperTask>().As<IStartupTask>();
            builder.RegisterType<PluginDirectoryCleanerTask>().As<IStartupTask>();

            // HTTP management interface
		    builder.RegisterModule(new HttpManagementModule());

			// Register plugin engine
		    builder.RegisterType<CmdArgPluginScanner>().As<IPluginScanner>();
		    builder.RegisterType<DirectoryPluginScanner>()
		        .As<IPluginScanner>()
		        .WithParameter(
		            (p, ctx) => p.ParameterType == typeof (string),
		            (p, ctx) => ctx.Resolve<IConfiguration>().Plugins.BaseDirectory);

            builder.RegisterType<PackageInstaller>()
                .As<IPackageInstaller>()
                .WithParameter(
                    (p, ctx) => p.ParameterType == typeof(string),
                    (p, ctx) => ctx.Resolve<IConfiguration>().Plugins.BaseDirectory);

		    builder.RegisterType<PluginManagerFactory>().As<IPluginManagerFactory>();
		    builder.RegisterType<PackageDownloader>().As<IPackageDownloader>();
		    builder.RegisterType<PackageReader>().As<IPackageReader>();
		    builder.RegisterType<IsolatedEnvironmentFactory>().As<IIsolatedEnvironmentFactory>();
			builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();

            // Messaging
		    builder.RegisterType<MessageQueue>().As<IMessageQueue>();
		    builder.RegisterType<PluginErrorHandler>().As<IMessageHandler<PluginErrorMessage>>();

			// Register file system
			builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
		    builder.RegisterType<RootPathProvider>().As<IRootPathProvider>().SingleInstance();

            // JSONRPC service stuff
		    builder.RegisterType<JsonRpcRequestParser>().As<IJsonRpcRequestParser>();
		    builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
		    builder.RegisterType<MethodCacheBuilder>().As<IMethodCacheBuilder>();
            builder.RegisterType<ByNameResolver>().As<IParameterResolver>();
            builder.RegisterType<ByPositionResolver>().As<IParameterResolver>();
            builder.RegisterType<NullResolver>().As<IParameterResolver>();
		    builder.RegisterType<JsonSerializer>().As<IJsonSerializer>();
		    builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();
		    builder.Register<IClientTransport>(c =>
		    {
		        var cfg = c.Resolve<IConfiguration>();
		        return new WcfClientTransport(cfg.Rpc.PluginUriTemplate, c.Resolve<IJsonSerializer>());
		    });

            // JSONRPC services
            builder.RegisterType<AuthService>().As<IJsonRpcService>();
            builder.RegisterType<EventsService>().As<IJsonRpcService>();
            builder.RegisterType<PluginsService>().As<IJsonRpcService>();
		    builder.RegisterType<LogService>().As<IJsonRpcService>();
		    builder.RegisterType<CoreService>().As<IJsonRpcService>();
		    builder.RegisterType<ReleasesService>().As<IJsonRpcService>();
		    builder.RegisterType<RepositoryService>().As<IJsonRpcService>();

            // Register wcf stuff
            builder.RegisterType<PluginService>();
		    builder.Register<IPluginServiceHost>(c =>
		    {
		        var scope = c.Resolve<ILifetimeScope>();
		        var cfg = c.Resolve<IConfiguration>();
		        var uri = new Uri(string.Format(cfg.Rpc.PluginUriTemplate, "core"));

                var binding = new NetHttpBinding
                {
                    HostNameComparisonMode = HostNameComparisonMode.Exact,
                    MaxBufferPoolSize = 10485760,
                    MaxReceivedMessageSize = 10485760,
                };

		        var host = new ServiceHost(typeof (PluginService));
		        host.AddServiceEndpoint(typeof (IPluginService), binding, uri);
                host.AddDependencyInjectionBehavior(typeof(PluginService), scope);

		        return new PluginServiceHost(host);
		    });

            // API connection
		    builder.RegisterType<ApiConnection>().As<IApiConnection>();
		    builder.RegisterType<PluginRepository>().As<IPluginRepository>();
		    builder.RegisterType<ReleasesRepository>().As<IReleasesRepository>();

			// Register configuration
			builder.Register(c => HadoukenConfigurationSection.Load()).SingleInstance();

            // Security
		    builder.RegisterType<AuthenticationManager>().As<IAuthenticationManager>();

            // Logging
		    builder.RegisterType<InMemorySink>().As<IInMemorySink>().SingleInstance();
		    builder.Register(c =>
		    {
		        var memSink = c.Resolve<IInMemorySink>();
		        return new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Sink(memSink)
                    .WriteTo.ColoredConsole()
                    .CreateLogger();
		    });

			// Resolve the service.
		    var container = builder.Build();
			return container.Resolve<IHadoukenService>();
		}
	}
}
