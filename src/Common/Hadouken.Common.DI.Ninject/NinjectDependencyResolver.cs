using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.DI.Ninject.Modules;
using Ninject;

namespace Hadouken.Common.DI.Ninject
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver()
        {
            _kernel = new StandardKernel(
                new ComponentModule(),
                new PluginModule(),
                new MessageHandlerModule()
            );
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public object Get(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _kernel.GetAll<T>();
        }

        public IEnumerable<object> GetAll(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        } 

        public void BindToFunc<T>(Func<T> factory)
        {
            _kernel.Bind<T>().ToMethod(ctx => factory());
        }

        public void BindToInstance<T>(T instance)
        {
            _kernel.Bind<T>().ToConstant(instance);
        }
    }
}
