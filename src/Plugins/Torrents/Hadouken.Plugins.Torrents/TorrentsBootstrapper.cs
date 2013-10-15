using System;
using System.IO;
using System.ServiceModel;
using Autofac.Core;
using Autofac.Integration.Wcf;
using Hadouken.Framework;
using Hadouken.Framework.DI;
using Hadouken.Framework.Events;
using Hadouken.Framework.Http;
using Hadouken.Framework.Http.Media;
using Hadouken.Framework.IO;
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
        public IContainer Container { get; private set; }
        
        public override Plugin Load(IBootConfig config)
        {
            Container = BuildContainer(config);
            return Container.Resolve<Plugin>();
        }

        private IContainer BuildContainer(IBootConfig config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new PluginModule());
            builder.RegisterModule(new JsonRpcServiceModule());
            builder.RegisterModule(new WcfJsonRpcServerModule(() => Container, config.RpcPluginUri));

            builder.RegisterType<OctoTorrentEngine>().As<IBitTorrentEngine>().SingleInstance();
            builder.RegisterType<EngineSettingsFactory>().As<IEngineSettingsFactory>().SingleInstance();

            var eventListenerUri = new Uri(String.Format("http://{0}:{1}/events", config.HostBinding, config.Port));

            builder.Register<IHttpFileServer>
                (c =>
                {
                    var mediaTypeFactory = new MediaTypeFactory(new FileSystem());

                    var server = new HttpFileServer(
                        String.Format("http://{0}:{1}{2}", config.HostBinding, config.Port, config.HttpVirtualPath)
                        , Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI"), mediaTypeFactory, new EventListener(eventListenerUri));

                    server.SetCredentials(config.UserName, config.Password);

                    return server;
                });

            builder.Register(c => config).SingleInstance();
            builder.RegisterType<JsonRpcClient>().As<IJsonRpcClient>();
            builder.Register<IClientTransport>(c => new WcfNamedPipeClientTransport(config.RpcGatewayUri)).SingleInstance();

            return builder.Build();
        }
    }
}
