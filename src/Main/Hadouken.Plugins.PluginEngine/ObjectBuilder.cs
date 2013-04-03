using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.DI;

namespace Hadouken.Plugins.PluginEngine
{
    [Serializable]
    public class ObjectBuilder
    {
        private readonly ProxyResolver _resolver;
        private IDictionary<string, object> _objectCache = new Dictionary<string, object>(); 

        public ObjectBuilder(IDependencyResolver resolver)
        {
            _resolver = new ProxyResolver(resolver);
        }

        public object Get(Type type)
        {
            var name = String.Format("{0}, {1}", type.FullName, type.Assembly.FullName);

            if (_objectCache.ContainsKey(name))
            {
                return _objectCache[name];
            }

            if (_resolver.Has(name))
            {
                var o = _resolver.Get(type);
                _objectCache.Add(name, o);

                return o;
            }

            object local = BuildFromCurrent(type);
            _objectCache.Add(name, local);

            return local;
        }

        private object BuildFromCurrent(Type type)
        {
            var implementingType = GetImplementingType(type);

            var constructor = implementingType.GetConstructors().First();
            var parameters = constructor.GetParameters().Select(p => Get(p.ParameterType)).ToArray();

            return constructor.Invoke(parameters);
        }

        private Type GetImplementingType(Type interfaceType)
        {
            return (from asm in AppDomain.CurrentDomain.GetAssemblies()
                    from type in asm.GetTypes()
                    where interfaceType.IsAssignableFrom(type)
                    where type.IsClass && !type.IsAbstract
                    select type).FirstOrDefault();
        }
    }
}
