using Hadouken.Framework;
using Hadouken.Framework.DI;
using Hadouken.Framework.Plugins;
using Hadouken.Plugins.Torrents.BitTorrent;
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
            builder.RegisterAssemblyModules<ParameterlessConstructorModule>(typeof (Bootstrapper).Assembly);
            builder.RegisterModule(new ConfigModule(config));
            builder.RegisterModule(new WcfJsonRpcServerModule(() => Container, config.RpcPluginUri));

            builder.RegisterType<OctoTorrentEngine>().As<IBitTorrentEngine>().SingleInstance();
            builder.RegisterType<EngineSettingsFactory>().As<IEngineSettingsFactory>().SingleInstance();

            return builder.Build();
        }
    }
}
