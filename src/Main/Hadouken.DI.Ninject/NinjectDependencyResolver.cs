using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.DI.Ninject.Modules;
using Ninject;
using Ninject.Activation;
using Ninject.Extensions.ChildKernel;

namespace Hadouken.DI.Ninject
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver()
        {
            _kernel = new StandardKernel(
                new ComponentModule()
            );
        }

        public object Get(Type t)
        {
            return _kernel.Get(t);
        }

        public object Get(Type t, string name)
        {
            return _kernel.Get(t, name);
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public T Get<T>(string name)
        {
            return _kernel.Get<T>(name);
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _kernel.GetAll<T>();
        }

        public T TryGet<T>()
        {
            return _kernel.TryGet<T>();
        }

        public T TryGet<T>(string name)
        {
            return _kernel.TryGet<T>(name);
        }

        public void Register(Type service, Type implementation)
        {
            _kernel.Bind(service).To(implementation).InSingletonScope();
        }

        public void Register(Type service, object constant)
        {
            _kernel.Bind(service).ToConstant(constant);
        }

        public void Register(Type service, Type implementation, string name)
        {
            _kernel.Bind(service).To(implementation).Named(name);
        }
    }
}
