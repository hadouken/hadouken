using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Serilog;

namespace Hadouken.Messaging
{
    public interface IMessageQueue
    {
        Task Publish<T>(T message) where T : Message;
    }

    public class MessageQueue : IMessageQueue
    {
        private readonly ILogger _logger;
        private readonly ILifetimeScope _lifetimeScope;

        public MessageQueue(ILogger logger, ILifetimeScope lifetimeScope)
        {
            _logger = logger;
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
                        _logger.Error(e, "Error when handling message: {Message}", message);
                    }
                }
            });
        }
    }
}
