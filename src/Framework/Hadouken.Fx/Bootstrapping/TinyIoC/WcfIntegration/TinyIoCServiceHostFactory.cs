using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Hadouken.Fx.Bootstrapping.TinyIoC.WcfIntegration
{
    public class TinyIoCServiceHostFactory : ServiceHostFactory
    {
        private readonly TinyIoCContainer _container;

        public TinyIoCServiceHostFactory(TinyIoCContainer container)
        {
            _container = container;
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new TinyIoCServiceHost(_container, serviceType, baseAddresses);
        }
    }
}
