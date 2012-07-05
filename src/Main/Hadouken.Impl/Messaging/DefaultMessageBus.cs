using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hadouken.Messaging;
using Castle.DynamicProxy;

namespace Hadouken.Impl.Messaging
{
    public class DefaultMessageBus : IMessageBus
    {
        private ProxyGenerator _generator = new ProxyGenerator();

        private static class Internal<T> where T : IMessage
        {
            private static List<Action<T>> _subscribers = new List<Action<T>>();

            public static IList<Action<T>> Subscribers { get { return _subscribers; } }
        }

        public Task Send<TMessage>(Action<TMessage> builder) where TMessage : class, IMessage
        {
            return Task.Factory.StartNew(() =>
            {
                // create dynamic proxy
                var msg = _generator.CreateInterfaceProxyWithoutTarget<TMessage>(new PropertiesInterceptor());

                builder(msg);

                foreach (var callback in Internal<TMessage>.Subscribers)
                {
                    callback(msg);
                }
            });
        }

        public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage
        {
            Internal<TMessage>.Subscribers.Add(callback);
        }
    }
}
