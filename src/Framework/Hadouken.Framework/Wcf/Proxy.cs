using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.Wcf
{
    public class Proxy<T> : IProxy<T>
    {
        private readonly Uri _endpoint;
        private readonly IBindingFactory _bindingFactory;
        private readonly Lazy<T> _lazyChannel;

        public Proxy(Uri endpoint, IBindingFactory bindingFactory)
        {
            _endpoint = endpoint;
            _bindingFactory = bindingFactory;
            _lazyChannel = new Lazy<T>(BuildChannel);
        }

        private T BuildChannel()
        {
            var binding = _bindingFactory.Create(_endpoint);
            var factory = new ChannelFactory<T>(binding, _endpoint.ToString());

            return factory.CreateChannel();
        }

        public T Channel
        {
            get { return _lazyChannel.Value; }
        }
    }
}
