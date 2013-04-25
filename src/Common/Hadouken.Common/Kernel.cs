using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.DI;

namespace Hadouken.Common
{
    public static class Kernel
    {
        private static IDependencyResolver _dependencyResolver;

        public static void SetResolver(IDependencyResolver dependencyResolver)
        {
            if(dependencyResolver == null)
                throw new ArgumentNullException("dependencyResolver");

            _dependencyResolver = dependencyResolver;
        }

        public static T Get<T>()
        {
            return _dependencyResolver.Get<T>();
        }

        public static object Get(Type serviceType)
        {
            return _dependencyResolver.Get(serviceType);
        }

        public static IEnumerable<T> GetAll<T>()
        {
            return _dependencyResolver.GetAll<T>();
        }

        public static IEnumerable<object> GetAll(Type serviceType)
        {
            return _dependencyResolver.GetAll(serviceType);
        } 

        public static void BindToFunc<T>(Func<T> factory)
        {
            _dependencyResolver.BindToFunc(factory);
        }

        public static void BindToInstance<T>(T instance)
        {
            _dependencyResolver.BindToInstance<T>(instance);
        }
    }
}
