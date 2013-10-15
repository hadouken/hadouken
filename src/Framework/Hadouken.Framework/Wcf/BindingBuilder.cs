using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Hadouken.Framework.Wcf
{
    public class BindingBuilder : IBindingBuilder
    {
        private readonly IDictionary<string, Func<string, Binding>> _bindings = new Dictionary<string, Func<string, Binding>>();

        public BindingBuilder()
        {
            _bindings.Add("net.pipe", BuildNamedPipesBinding);
        }

        public Binding Build(string bindingUri)
        {
            if (String.IsNullOrEmpty(bindingUri))
                throw new ArgumentNullException("bindingUri");

            var uri = new Uri(bindingUri);
            var scheme = uri.Scheme;

            if (_bindings.ContainsKey(scheme))
                return _bindings[scheme](bindingUri);

            throw new ArgumentException("No binding registered for scheme " + scheme, "bindingUri");
        }

        private static Binding BuildNamedPipesBinding(string uri)
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