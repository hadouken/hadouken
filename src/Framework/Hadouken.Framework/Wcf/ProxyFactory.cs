using System;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.Wcf
{
    public class ProxyFactory<T> : IProxyFactory<T>
    {
        private readonly IBindingFactory _bindingFactory;

        public ProxyFactory(IBindingFactory bindingFactory)
        {
            _bindingFactory = bindingFactory;
        } 

        public IProxy<T> Create(Uri endpoint)
        {
            return new Proxy<T>(endpoint, _bindingFactory);
        }
    }
}