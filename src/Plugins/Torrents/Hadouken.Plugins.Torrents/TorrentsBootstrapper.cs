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
            builder.RegisterModule(new ConfigModule(config));
            builder.RegisterModule(new PluginModule());
            builder.RegisterModule(new JsonRpcServiceModule());
            builder.RegisterModule(new WcfJsonRpcServerModule(() => Container, config.RpcPluginUri));
            builder.RegisterModule(new FileSystemModule());
            builder.RegisterModule(new JsonRpcClientModule());
            builder.RegisterModule(new EventListenerModule());

            var httpListenUri = String.Format("http://{0}:{1}{2}", config.HostBinding, config.Port, config.HttpVirtualPath);
            var httpBaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UI");
            builder.RegisterModule(new HttpFileServerModule(httpListenUri, httpBaseDirectory));

            builder.RegisterType<OctoTorrentEngine>().As<IBitTorrentEngine>().SingleInstance();
            builder.RegisterType<EngineSettingsFactory>().As<IEngineSettingsFactory>().SingleInstance();

            return builder.Build();
        }
    }
}
