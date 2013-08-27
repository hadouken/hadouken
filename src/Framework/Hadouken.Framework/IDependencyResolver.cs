using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadouken.Framework
{
    public interface IDependencyResolver
    {
        void Register<TService>(Type implementation);

        void RegisterAll<TService>();

        void Register<TService, TImplementation>() where TImplementation : TService;

        void Register<TService>(Func<TService> factory);
    }
}
