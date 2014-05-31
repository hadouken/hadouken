using System;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using Hadouken.Configuration;
using Hadouken.Fx.ServiceModel;

namespace Hadouken.DI.Modules
{
    public sealed class PluginServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PluginService>();
            builder.Register<IPluginServiceHost>(c =>
            {
                var scope = c.Resolve<ILifetimeScope>();
                var cfg = c.Resolve<IConfiguration>();
                var uri = new Uri(string.Format(cfg.PluginUrlTemplate, "core"));

                var binding = new NetHttpBinding
                {
                    HostNameComparisonMode = HostNameComparisonMode.Exact,
                    MaxBufferPoolSize = 10485760,
                    MaxReceivedMessageSize = 10485760,
                };

                var host = new ServiceHost(typeof(PluginService));
                host.AddServiceEndpoint(typeof(IPluginService), binding, uri);
                host.AddDependencyInjectionBehavior(typeof(PluginService), scope);

                return new PluginServiceHost(host);
            });
        }
    }
}
