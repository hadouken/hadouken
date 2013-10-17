using Hadouken.Framework;
using Hadouken.Plugins.Torrents.BitTorrent;
using Autofac;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<OctoTorrentEngine>().As<IBitTorrentEngine>().SingleInstance();
            builder.RegisterType<EngineSettingsFactory>().As<IEngineSettingsFactory>().SingleInstance();
        }
    }
}
