using System.Net;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Configuration;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.IO;
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

			// Register RPC services
			builder.RegisterType<PluginsService>().As<IJsonRpcService>();
		    builder.RegisterType<EventsService>().As<IJsonRpcService>();
		    builder.RegisterType<CoreServices>().As<IJsonRpcService>();
			builder.RegisterType<WcfProxyRequestHandler>().As<IRequestHandler>();
			builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>();

		    builder.Register(c =>
		    {
		        var handler = c.Resolve<IJsonRpcHandler>();
		        return new WcfJsonRpcServer("net.pipe://localhost/hdkn.jsonrpc", handler);
		    });

		    builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();
		    builder.Register<IClientTransport>(c => new WcfNamedPipeClientTransport("net.pipe://localhost/hdkn.jsonrpc"));

			// Register JSONRPC server
			builder.Register<IHttpJsonRpcServer>(c =>
			{
				var conf = c.Resolve<IConfiguration>();
				var uri = String.Format("http://{0}:{1}/jsonrpc/", conf.Http.HostBinding, conf.Http.Port);

			    NetworkCredential credentials = null;

			    if (!String.IsNullOrEmpty(conf.Http.Authentication.UserName) &&
			        !String.IsNullOrEmpty(conf.Http.Authentication.Password))
			    {
			        credentials = new NetworkCredential(conf.Http.Authentication.UserName, conf.Http.Authentication.Password);
			    }

			    return new HttpJsonRpcServer(uri, credentials);
			});

            // Register SignalR event server
		    builder.RegisterType<EventServer>().As<IEventServer>().SingleInstance();

			// Register configuration
			builder.Register(c => ApplicationConfigurationSection.Load()).SingleInstance();

			// Build the container.
			var container = builder.Build();

			// Resolve the service.
			return container.Resolve<IHadoukenService>();
		}
	}
}
