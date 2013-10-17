using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Hadouken.Framework.Wcf
{
    public class BindingFactory : IBindingFactory
    {
        private readonly IDictionary<string, Func<Uri, Binding>> _bindings = new Dictionary<string, Func<Uri, Binding>>();

        public BindingFactory()
        {
            _bindings.Add("net.pipe", BuildNamedPipesBinding);
        }

        public Binding Create(Uri listenUri)
        {
            var scheme = listenUri.Scheme;

            if (_bindings.ContainsKey(scheme))
                return _bindings[scheme](listenUri);

            throw new ArgumentException("No binding registered for scheme " + scheme, "listenUri");
        }

        private static Binding BuildNamedPipesBinding(Uri listenUri)
        {
            return new NetNamedPipeBinding()
            {
                MaxBufferPoolSize = 10485760,
                MaxBufferSize = 10485760,
                MaxConnections = 10,
                MaxReceivedMessageSize = 10485760
            };
        }
    }
}