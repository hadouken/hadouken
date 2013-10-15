using Hadouken.Framework;
using Hadouken.Framework.DI;
using Hadouken.Framework.Plugins;
using Hadouken.Plugins.Config.Data;
using Autofac;

namespace Hadouken.Plugins.Config
{
    public class ConfigBootstrapper : Bootstrapper
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
            builder.RegisterAssemblyModules<ParameterlessConstructorModule>(typeof(Bootstrapper).Assembly);
            builder.RegisterModule(new WcfJsonRpcServerModule(() => Container, config.RpcPluginUri));

            // Data store
            builder.Register<IConfigDataStore>(c => new SQLiteDataStore(config.DataPath)).SingleInstance();

            return builder.Build();
        }
    }
}
