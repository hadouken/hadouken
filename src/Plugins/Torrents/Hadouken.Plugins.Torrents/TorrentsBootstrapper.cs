using System;
using System.IO;
using System.ServiceModel;
using Autofac.Integration.Wcf;
using Hadouken.Framework;
using Hadouken.Framework.Http;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Torrents.BitTorrent;
using Hadouken.Plugins.Torrents.Rpc;
using Autofac;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = BuildContainer(config);
            return container.Resolve<Plugin>();
        }

        private static IContainer BuildContainer(IBootConfig config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TorrentsPlugin>().As<Plugin>();
            builder.RegisterType<MonoTorrentEngine>().As<IBitTorrentEngine>().SingleInstance();
            builder.RegisterType<EngineSettingsFactory>().As<IEngineSettingsFactory>().SingleInstance();

            builder.Register<IHttpFileServer>
                (c =>
                 new HttpFileServer(
                     String.Format("http://{0}:{1}{2}", config.HostBinding, config.Port, config.HttpVirtualPath)
                     , Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI"),
                     config.HttpVirtualPath));

            builder.Register(c => config).SingleInstance();

            builder.RegisterType<TorrentsServices>().As<IJsonRpcService>().SingleInstance();
            builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();
            builder.RegisterType<WcfJson>().As<IWcfJsonRpcServer>();
            builder.Register<IClientTransport>(c => new WcfNamedPipeClientTransport(config.GatewayRpcBinding)).SingleInstance();

            var container = builder.Build();
            var wcfBuilder = new ContainerBuilder();

            // Register WCF
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
                host.AddServiceEndpoint(typeof(IWcfJsonRpcServer), binding, config.PluginRpcBinding);
                host.AddDependencyInjectionBehavior<IWcfJsonRpcServer>(container);

                return new WcfJsonRpcServer(host);
            });

            wcfBuilder.Update(container);

            return container;
        }
    }
}
