using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.DI
{
    public interface IDependencyResolver
    {
        object Get(Type t);
        object Get(Type t, string name);

        T Get<T>();
        T Get<T>(string name);

        IEnumerable<T> GetAll<T>();

        T TryGet<T>();
        T TryGet<T>(string name);

        void Register(Type service, Type implementation);
        void Register(Type service, Type implementation, string name);

        void Register(Type service, object constant);
    }
}
