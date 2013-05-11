using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hadouken.Messaging;
using Castle.DynamicProxy;
using NLog;

namespace Hadouken.Impl.Messaging
{
    [Component]
    public class DefaultMessageBus : IMessageBus
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ProxyGenerator _generator = new ProxyGenerator();

        private static class Internal<T> where T : IMessage
        {
            private static readonly List<Action<T>> _subscribers = new List<Action<T>>();

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
                    try
                    {
                        callback(msg);
                    }
                    catch(Exception e)
                    {
                        Logger.ErrorException(String.Format("Could not execute message handler."), e);
                    }
                }
            });
        }

        public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage
        {
            if (callback.Method.DeclaringType != null)
                Logger.Trace(String.Format("Adding subscription to message {0} from {1}", typeof(TMessage), callback.Method.DeclaringType.FullName + "." + callback.Method.Name));

            Internal<TMessage>.Subscribers.Add(callback);
        }
    }
}
