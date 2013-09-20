using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.NoSql.Rpc;
using InjectMe;
using InjectMe.Registration;

namespace Hadouken.Plugins.NoSql
{
    public class NoSqlBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = Container.Create(cfg => BuildContainer(config, cfg));
            return container.ServiceLocator.Resolve<Plugin>();
        }

        private static void BuildContainer(IBootConfig bootConfig, IContainerConfiguration containerConfig)
        {
            containerConfig.Register<Plugin>().AsSingleton().UsingConcreteType<NoSqlPlugin>();

            containerConfig.Register<IConfigStore>().AsSingleton().UsingFactory(() => new ConfigStore(":in-memory:"));

            // Register JSONRPC stuffs
            containerConfig.Register<IJsonRpcService>().AsSingleton().UsingConcreteType<ConfigService>();
            containerConfig.Register<IJsonRpcHandler>().AsSingleton().UsingConcreteType<JsonRpcHandler>();

            // Register WCF json rpc server
            containerConfig.Register<IJsonRpcServer>().AsSingleton().UsingFactory(ac =>
            {
                var handler = ac.Container.ServiceLocator.Resolve<IJsonRpcHandler>();
                return new WcfJsonRpcServer("net.pipe://localhost/hdkn.plugins.core.nosql", handler);
            });
        }
    }
}
