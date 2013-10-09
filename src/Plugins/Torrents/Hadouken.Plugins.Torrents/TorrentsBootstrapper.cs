using System;
using System.IO;
using Hadouken.Framework;
using Hadouken.Framework.Http;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Torrents.BitTorrent;
using Hadouken.Plugins.Torrents.Rpc;
using InjectMe;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = Container.Create(cfg =>
            {
                var uri = String.Format("http://{0}:{1}{2}", config.HostBinding, config.Port, config.HttpVirtualPath);

                cfg.Register<IHttpFileServer>()
                    .AsSingleton()
                    .UsingFactory(
                        () => new HttpFileServer(uri, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI"),
                            config.HttpVirtualPath));

                cfg.Register<IBootConfig>().AsSingleton().UsingFactory(c => config);

                cfg.Register<IJsonRpcService>().AsSingleton().UsingConcreteType<TorrentsServices>();
                cfg.Register<IJsonRpcHandler>().AsSingleton().UsingConcreteType<JsonRpcHandler>();
                cfg.Register<IRequestHandler>().AsSingleton().UsingConcreteType<RequestHandler>();

                cfg.Register<IJsonRpcServer>().AsSingleton().UsingFactory(context =>
                {
                    var handler = context.Container.ServiceLocator.Resolve<IJsonRpcHandler>();
                    return new WcfJsonRpcServer(config.PluginRpcBinding, handler);
                });

                cfg.Register<IClientTransport>()
                    .AsSingleton()
                    .UsingFactory(c => new WcfNamedPipeClientTransport(config.GatewayRpcBinding));

                cfg.Register<IJsonRpcClient>().AsSingleton().UsingConcreteType<JsonRpcClient>();

                cfg.Register<IBitTorrentEngine>()
                    .AsSingleton()
                    .UsingConcreteType<MonoTorrentEngine>();

                cfg.Register<Plugin>().AsSingleton().UsingConcreteType<TorrentsPlugin>();
            });


            return container.ServiceLocator.Resolve<Plugin>();
        }
    }
}
