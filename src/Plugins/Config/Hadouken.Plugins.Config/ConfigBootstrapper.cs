using System;
using System.IO;
using System.ServiceModel;
using Autofac.Integration.Wcf;
using Hadouken.Framework;
using Hadouken.Framework.Plugins;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Plugins.Config.Rpc;

using Hadouken.Framework.Rpc;
using Hadouken.Plugins.Config.Data;
using Autofac;

namespace Hadouken.Plugins.Config
{
    public class ConfigBootstrapper : Bootstrapper
    {
        public override Plugin Load(IBootConfig config)
        {
            var container = BuildContainer(config);
            return container.Resolve<Plugin>();
        }

        private static IContainer BuildContainer(IBootConfig config)
        {
            var builder = new ContainerBuilder();

            // Register plugin
            builder.RegisterType<ConfigPlugin>().As<Plugin>();

            // JSONRPC
            builder.RegisterType<ConfigService>().As<IJsonRpcService>().SingleInstance();
            builder.RegisterType<JsonRpcHandler>().As<IJsonRpcHandler>().SingleInstance();
            builder.RegisterType<RequestHandler>().As<IRequestHandler>().SingleInstance();
            builder.RegisterType<WcfJson>().As<IWcfJsonRpcServer>();

            // Data store
            builder.Register<IConfigDataStore>(c => new SQLiteDataStore(config.DataPath)).SingleInstance();

            var container = builder.Build();
            var wcfBuilder = new ContainerBuilder();

            // Register WCF
            wcfBuilder.Register(c =>
            {
                var binding = new NetNamedPipeBinding
                {
                    MaxBufferPoolSize = 10485760,
                    MaxBufferSize = 10485760,
                    MaxConnections = 10,
                    MaxReceivedMessageSize = 10485760
                };

                var host = new ServiceHost(typeof(WcfJson));
                host.AddServiceEndpoint(typeof(IWcfJsonRpcServer), binding, config.PluginRpcBinding);
                host.AddDependencyInjectionBehavior<IWcfJsonRpcServer>(container);

                return new WcfJsonRpcServer(host);
            });

            wcfBuilder.Update(container);

            return container;
        }
    }
}
