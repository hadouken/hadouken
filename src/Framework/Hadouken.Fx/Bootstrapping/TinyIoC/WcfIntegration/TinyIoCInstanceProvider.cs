using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Hadouken.Fx.Bootstrapping.TinyIoC.WcfIntegration
{
    public class TinyIoCInstanceProvider : IInstanceProvider
    {
        private readonly TinyIoCContainer _container;
        private readonly Type _serviceType;

        public TinyIoCInstanceProvider(TinyIoCContainer container, Type serviceType)
        {
            _container = container;
            _serviceType = serviceType;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return _container.Resolve(_serviceType);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance) { }
    }
}
