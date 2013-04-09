using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.DI
{
    public interface IDependencyResolver
    {
        T Get<T>();
        IEnumerable<T> GetAll<T>();

        void BindToFunc<T>(Func<T> factory);

        void BindToInstance<T>(T instance);
    }
}
