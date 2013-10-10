using System.Net;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Configuration;
using Hadouken.Framework.Events;
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
            builder.RegisterType<PluginsService>().As<IJsonRpcService>().SingleInstance();
            builder.RegisterType<EventsService>().As<IJsonRpcService>().SingleInstance();
		    builder.RegisterType<CoreServices>().As<IJsonRpcService>().SingleInstance();
			builder.RegisterType<WcfProxyRequestHandler>().As<IRequestHandler>().SingleInstance();
			builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();

		    builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();
		    builder.Register<IClientTransport>(c => new WcfNamedPipeClientTransport("net.pipe://localhost/hdkn.jsonrpc"));

			// Register JSONRPC server
		    builder.RegisterType<WcfJson>().As<IWcfJsonRpcServer>();
			builder.Register<IHttpJsonRpcServer>(c =>
			{
				var conf = c.Resolve<IConfiguration>();
			    var eventListener = c.Resolve<IEventListener>();

				var uri = String.Format("http://{0}:{1}/jsonrpc/", conf.Http.HostBinding, conf.Http.Port);

			    var server = new HttpJsonRpcServer(uri, eventListener);
			    server.SetCredentials(conf.Http.Authentication.UserName, conf.Http.Authentication.Password);

			    return server;
			});

            // Register SignalR event server
		    builder.RegisterType<EventServer>().As<IEventServer>().SingleInstance();

		    builder.Register<IEventListener>(c =>
		    {
		        var conf = c.Resolve<IConfiguration>();
		        var eventListenerUri =
		            new Uri(String.Format("http://{0}:{1}/", conf.Http.HostBinding, conf.Http.Port + 1));

		        return new EventListener(eventListenerUri);
		    });

			// Register configuration
			builder.Register(c => ApplicationConfigurationSection.Load()).SingleInstance();

			// Build the container.
			var container = builder.Build();
            
            // Register the WCF host
		    var wcfBuilder = new ContainerBuilder();
            wcfBuilder.Register(c =>
            {
                var binding = new NetNamedPipeBinding
                {
                    MaxBufferPoolSize = 10485760,
                    MaxBufferSize = 10485760,
                    MaxConnections = 10,
                    MaxReceivedMessageSize = 10485760
                };

                var host = new ServiceHost(typeof(WcfJson));
                host.AddServiceEndpoint(typeof(IWcfJsonRpcServer), binding, "net.pipe://localhost/hdkn.jsonrpc");
                host.AddDependencyInjectionBehavior<IWcfJsonRpcServer>(container);

                return new WcfJsonRpcServer(host);
            });

		    wcfBuilder.Update(container);

			// Resolve the service.
			return container.Resolve<IHadoukenService>();
		}
	}
}
