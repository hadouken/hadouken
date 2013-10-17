using Hadouken.Framework;
using Hadouken.Framework.DI;
using Hadouken.Framework.Plugins;
using Hadouken.Plugins.Config.Data;
using Autofac;

namespace Hadouken.Plugins.Config
{
    public class ConfigBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder builder)
        {
            builder.Register<IConfigDataStore>(c => new SQLiteDataStore(Configuration.DataPath)).SingleInstance();
        }
    }
}
