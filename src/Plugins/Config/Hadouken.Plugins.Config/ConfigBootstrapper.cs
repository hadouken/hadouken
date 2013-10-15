using System;
using System.IO;
using System.ServiceModel;
using Autofac.Integration.Wcf;
using Hadouken.Framework;
using Hadouken.Framework.DI;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Framework.Wcf;
using Hadouken.Plugins.Config.Rpc;

using Hadouken.Framework.Rpc;
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
            builder.RegisterModule(new PluginModule());
            builder.RegisterModule(new JsonRpcServiceModule());
            builder.RegisterModule(new WcfJsonRpcServerModule(() => Container, config.RpcPluginUri));

            // Data store
            builder.Register<IConfigDataStore>(c => new SQLiteDataStore(config.DataPath)).SingleInstance();

            return builder.Build();
        }
    }
}
