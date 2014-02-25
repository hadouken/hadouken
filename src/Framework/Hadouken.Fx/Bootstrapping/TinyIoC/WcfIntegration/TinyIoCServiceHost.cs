using System;
using System.ServiceModel;

namespace Hadouken.Fx.Bootstrapping.TinyIoC.WcfIntegration
{
    public class TinyIoCServiceHost : ServiceHost
    {
        private readonly TinyIoCContainer _container;

        public TinyIoCServiceHost(TinyIoCContainer container)
        {
            _container = container;
        }

        public TinyIoCServiceHost(TinyIoCContainer container, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            _container = container;
        }

        protected override void OnOpening()
        {
            Description.Behaviors.Add(new TinyIoCServiceBehavior(_container));
            base.OnOpening();
        }
    }
}
