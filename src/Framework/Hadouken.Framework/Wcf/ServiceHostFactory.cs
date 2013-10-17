using System;
using System.Linq;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;

namespace Hadouken.Framework.Wcf
{
    public class ServiceHostFactory<T> : IServiceHostFactory<T>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IBindingFactory _bindingFactory;

        public ServiceHostFactory(ILifetimeScope lifetimeScope, IBindingFactory bindingFactory)
        {
            _lifetimeScope = lifetimeScope;
            _bindingFactory = bindingFactory;
        }

        public IServiceHost Create(Uri listenEndpoint)
        {
            // Find concrete type which implements the TContract type
            var serviceType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                from type in asm.GetTypes()
                where typeof (T).IsAssignableFrom(type)
                where type.IsClass && !type.IsAbstract
                select type).First();

            return Create(listenEndpoint, serviceType);
        }

        public IServiceHost Create(Uri listenEndpoint, Type serviceType)
        {
            var binding = _bindingFactory.Create(listenEndpoint);
            var host = new ServiceHost(serviceType);

            host.AddServiceEndpoint(typeof(T), binding, listenEndpoint);
            host.AddDependencyInjectionBehavior(typeof(T), _lifetimeScope);

            return new GenericServiceHost(host);
        }
    }
}