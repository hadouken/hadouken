using Hadouken.Framework;
using Hadouken.Plugins.Torrents.BitTorrent;
using Autofac;

namespace Hadouken.Plugins.Torrents
{
    public class TorrentsBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder builder)
        {
            string path = this.Configuration.DataPath;

            builder.RegisterType<OctoTorrentEngine>().As<IBitTorrentEngine>().SingleInstance();
            builder.RegisterType<EngineSettingsFactory>().As<IEngineSettingsFactory>().SingleInstance();
        }
    }
}
