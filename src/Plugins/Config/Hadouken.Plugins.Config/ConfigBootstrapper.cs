using Hadouken.Framework;
using Hadouken.Plugins.Config.Data;
using Autofac;

namespace Hadouken.Plugins.Config
{
    public class ConfigBootstrapper : DefaultBootstrapper
    {
        public override void RegisterDependencies(ContainerBuilder builder)
        {            
            builder.Register<IConfigDataStore>(c => new JsonDataStore(Configuration.DataPath)).SingleInstance();
        }
    }
}
