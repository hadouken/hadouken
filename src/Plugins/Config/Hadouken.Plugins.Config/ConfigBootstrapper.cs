using System;
using System.IO;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Config.Rpc;
using InjectMe.Registration;
using InjectMe;
using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Config.Data;

namespace Hadouken.Plugins.Config
{
    public class ConfigBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = Container.Create(_ => BuildConfiguration(config, _));
            return container.ServiceLocator.Resolve<Plugin>();
        }

        private static void BuildConfiguration(IBootConfig bootConfig, IContainerConfiguration containerConfiguration)
        {
            // Register plugin
            containerConfiguration.Register<Plugin>().AsSingleton().UsingConcreteType<ConfigPlugin>();

            // Register JSON RPC services and handlers
            containerConfiguration.Register<IJsonRpcService>().AsSingleton().UsingConcreteType<ConfigService>();
            containerConfiguration.Register<IJsonRpcHandler>().AsSingleton().UsingConcreteType<JsonRpcHandler>();
            containerConfiguration.Register<IRequestHandler>().AsSingleton().UsingConcreteType<RequestHandler>();

            // Register WCF json rpc server
            containerConfiguration.Register<IJsonRpcServer>().AsSingleton().UsingFactory(context =>
                {
                    var handler = context.Container.ServiceLocator.Resolve<IJsonRpcHandler>();
                    return new WcfJsonRpcServer(bootConfig.PluginRpcBinding, handler);
                });

            containerConfiguration.Register<IConfigDataStore>().AsSingleton().UsingFactory(c => new SQLiteDataStore(bootConfig.DataPath));
        }
    }
}
