using System;
using System.Reflection;
using Autofac;
using Hadouken.Configuration;
using Hadouken.Configuration.AppConfig;
using Hadouken.Fx.IO;
using Hadouken.Http;
using Hadouken.Http.Management;
using Hadouken.Plugins;
using Hadouken.Plugins.Isolation;
using Hadouken.Plugins.Repository;
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
		    builder.RegisterModule(new ManagementModule());

			// Register plugin engine
		    builder.RegisterType<CmdArgPluginScanner>().As<IPluginScanner>();
		    builder
		        .RegisterType<DirectoryPluginScanner>()
		        .As<IPluginScanner>()
		        .WithParameter(
		            (p, ctx) => p.ParameterType == typeof (string),
		            (p, ctx) => ctx.Resolve<IConfiguration>().Plugins.BaseDirectory);

		    builder.RegisterType<PluginManagerFactory>().As<IPluginManagerFactory>();
		    builder.RegisterType<PackageDownloader>().As<IPackageDownloader>();
		    builder.RegisterType<PackageReader>().As<IPackageReader>();
		    builder.RegisterType<IsolatedEnvironmentFactory>().As<IIsolatedEnvironmentFactory>();
			builder.RegisterType<PluginEngine>().As<IPluginEngine>().SingleInstance();

			// Register file system
			builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
		    builder.RegisterType<RootPathProvider>().As<IRootPathProvider>().SingleInstance();

            // API connection
		    builder.RegisterType<ApiConnection>().As<IApiConnection>();

			// Register configuration
			builder.Register(c => HadoukenConfigurationSection.Load()).SingleInstance();

			// Resolve the service.
		    var container = builder.Build();
			return container.Resolve<IHadoukenService>();
		}
	}
}
