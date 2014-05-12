using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using NLog;

namespace Hadouken.Messaging
{
    public interface IMessageQueue
    {
        Task Publish<T>(T message) where T : Message;
    }

    public class MessageQueue : IMessageQueue
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ILifetimeScope _lifetimeScope;

        public MessageQueue(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public Task Publish<T>(T message) where T : Message
        {
            return Task.Run(() =>
            {
                var handlers = _lifetimeScope.Resolve<IEnumerable<IMessageHandler<T>>>();

                foreach (var handler in handlers)
                {
                    try
                    {
                        handler.Handle(message);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(string.Format("Error when handling message {0}.", typeof (T).FullName), e);
                    }
                }
            });
        }
    }
}
