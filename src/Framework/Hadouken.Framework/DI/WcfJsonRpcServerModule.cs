using System;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using Hadouken.Framework.Rpc.Hosting;
using Hadouken.Framework.Wcf;

namespace Hadouken.Framework.DI
{
    public class WcfJsonRpcServerModule : Module
    {
        private readonly Func<IContainer> _containerFactory;
        private readonly string _bindingUri;

        public WcfJsonRpcServerModule(Func<IContainer> containerFactory, string bindingUri)
        {
            _containerFactory = containerFactory;
            _bindingUri = bindingUri;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BindingBuilder>().As<IBindingBuilder>().SingleInstance();

            builder.Register<IWcfJsonRpcServer>(c =>
            {
                var bindingBuilder = c.Resolve<IBindingBuilder>();
                var binding = bindingBuilder.Build(_bindingUri);

                var host = new ServiceHost(typeof (WcfJsonRpcService));
                host.AddServiceEndpoint(typeof (IWcfRpcService), binding, _bindingUri);
                host.AddDependencyInjectionBehavior<IWcfRpcService>(_containerFactory());

                return new WcfJsonRpcServer(host);
            });
        }
    }
}
