using System;
using System.ServiceModel;
using Hadouken.Framework.Rpc;

namespace Hadouken.Framework.Wcf
{
    public class Proxy<T> : IProxy<T>
    {
        private readonly ChannelFactory<T> _channelFactory;
 
        public Proxy(Uri endpoint, IBindingFactory bindingFactory)
        {
            var binding = bindingFactory.Create(endpoint);
            _channelFactory = new ChannelFactory<T>(binding, endpoint.ToString());
        }

        public T Channel
        {
            get { return _channelFactory.CreateChannel(); }
        }

        public void Dispose()
        {
            _channelFactory.Close();
        }
    }
}
