using System;
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
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Scanners;

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
		    builder.RegisterType<HttpBackendServer>().As<IHttpBackendServer>();

			// Register plugin engine
		    builder.RegisterType<CmdArgPluginScanner>().As<IPluginScanner>();
		    builder
		        .RegisterType<DirectoryPluginScanner>()
		        .As<IPluginScanner>()
		        .WithParameter(
		            (p, ctx) => p.ParameterType == typeof (string),
		            (p, ctx) => ctx.Resolve<IConfiguration>().Plugins.BaseDirectory);

            builder
                .RegisterType<PackageInstaller>()
                .As<IPackageInstaller>()
                .WithParameter(
                    (p, ctx) => p.ParameterType == typeof(string),
                    (p, ctx) => ctx.Resolve<IConfiguration>().Plugins.BaseDirectory);

		    builder.RegisterType<PluginManagerFactory>().As<IPluginManagerFactory>();
		    builder.RegisterType<PackageDownloader>().As<IPackageDownloader>();
		    builder.RegisterType<PackageReader>().As<IPackageReader>();
		    builder.RegisterType<IsolatedEnvironmentFactory>().As<IIsolatedEnvironmentFactory>();
			builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();

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
		    builder.RegisterType<WcfClientTransport>()
		        .As<IClientTransport>()
		        .WithParameter("uriTemplate", "net.pipe://localhost/hadouken.plugins.{0}");

            // JSONRPC services
            builder.RegisterType<AuthService>().As<IJsonRpcService>();
            builder.RegisterType<EventsService>().As<IJsonRpcService>();
            builder.RegisterType<PluginsService>().As<IJsonRpcService>();
		    builder.RegisterType<LogService>().As<IJsonRpcService>();
		    builder.RegisterType<CoreService>().As<IJsonRpcService>();

            // Register wcf stuff
            builder.RegisterType<PluginService>();
		    builder.Register<IPluginServiceHost>(c =>
		    {
		        var scope = c.Resolve<ILifetimeScope>();

		        var host = new ServiceHost(typeof (PluginService), new Uri("net.pipe://localhost/hadouken.plugins.core"));
                host.AddDependencyInjectionBehavior(typeof(PluginService), scope);

		        return new PluginServiceHost(host);
		    });

            // API connection
		    builder.RegisterType<ApiConnection>().As<IApiConnection>();
		    builder.RegisterType<PluginRepository>().As<IPluginRepository>();
		    builder.RegisterType<ReleasesRepository>().As<IReleasesRepository>();

			// Register configuration
			builder.Register(c => HadoukenConfigurationSection.Load()).SingleInstance();

			// Resolve the service.
		    var container = builder.Build();
			return container.Resolve<IHadoukenService>();
		}
	}
}
