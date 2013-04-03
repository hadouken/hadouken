using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;
using Hadouken.DI;

namespace Hadouken.Plugins.PluginEngine
{
    public class ProxyResolver : MarshalByRefObject
    {
        private readonly IDependencyResolver _resolver;
        private readonly ProxyGenerator _generator = new ProxyGenerator();

        public ProxyResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public bool Has(string typeName)
        {
            return _resolver.Has(typeName);
        }

        public object Get(Type type)
        {
            var o = _resolver.Get(type);

            return _generator.CreateInterfaceProxyWithTarget(type, o,
                                                             new ProxyGenerationOptions()
                                                                 {
                                                                     BaseTypeForInterfaceProxy =
                                                                         typeof (MarshalByRefObject)
                                                                 });
        }
    }
}
