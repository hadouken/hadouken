using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;

namespace Hadouken.Common.DI.Ninject
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver()
        {
            _kernel = new StandardKernel(
            );
        }

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _kernel.GetAll<T>();
        }

        public void BindToFunc<T>(Func<T> factory)
        {
            _kernel.Bind<T>().ToMethod(ctx => factory());
        }
    }
}
